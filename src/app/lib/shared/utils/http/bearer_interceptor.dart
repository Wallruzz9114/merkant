import 'dart:convert';

import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import 'package:dio/io.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_modular/flutter_modular.dart';
import 'package:merkant/models/access_token.dart';
import 'package:merkant/models/response_error.dart';
import 'package:merkant/services/interfaces/i_auth_service.dart';

class BearerInterceptor extends InterceptorsWrapper {
  @override
  Future<void> onRequest(
      RequestOptions options, RequestInterceptorHandler handler) async {
    final IAuthService authService = Modular.get<IAuthService>();

    if (await authService.isAuthenticated()) {
      final AccessToken? token = await authService.getCurrentToken();
      final String authHeader = generateToken(token!.refreshToken);
      options.headers['Authorization'] = authHeader;
    }

    super.onRequest(options, handler);
  }

  @override
  Future<void> onError(
    DioException err,
    ErrorInterceptorHandler handler,
  ) async {
    final IAuthService authService = Modular.get<IAuthService>();

    try {
      if (err.response?.statusCode == 401) {
        if (await authService.isAuthenticated()) {
          final AccessToken? token = await authService.getCurrentToken();
          final Either<ResponseFailure, AccessToken> refreshToken =
              await authService.refreshToken(token!.refreshToken);

          refreshToken.fold(
            (_) {
              if (kDebugMode)
                debugPrint(json.encode(
                    '========Erro RefreshToken - Redirect Login========'));
              authService.logout();
            },
            (AccessToken token) async {
              if (kDebugMode) {
                debugPrint(
                    json.encode('========Success Refresh Token========'));
                debugPrint('New Token: ${token.accessToken}');
              }

              authService.setCurrentToken(token);
              err.requestOptions.headers['Authorization'] =
                  generateToken(token.accessToken);

              try {
                final DioForNative dio = Modular.get<DioForNative>();
                final Response<dynamic> response =
                    await dio.fetch(err.requestOptions);
                handler.resolve(response);
              } catch (e) {
                handler.reject(err);
              }
            },
          );
        } else {
          authService.logout();
        }
      } else {
        handler.next(err);
      }
    } catch (e) {
      authService.logout();
    }
  }

  String generateToken(String token) => 'Bearer $token';
}
