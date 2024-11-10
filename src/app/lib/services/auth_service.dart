import 'dart:convert';

import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import 'package:dio/io.dart';
import 'package:flutter_modular/flutter_modular.dart';
import 'package:merkant/models/access_token.dart';
import 'package:merkant/models/login_input.dart';
import 'package:merkant/models/response_error.dart';
import 'package:merkant/models/user_account.dart';
import 'package:merkant/services/interfaces/i_auth_service.dart';
import 'package:merkant/services/local_storage_service.dart';
import 'package:merkant/shared/utils/http/error_interceptor.dart';
import 'package:merkant/shared/utils/http/log_interceptor.dart';

class AuthService implements IAuthService {
  static const String _currentToken = 'current_token';

  late final DioForNative dio;
  late final Dio dioWithoutJwt;

  AuthService() {
    dioWithoutJwt = Dio(Modular.get<BaseOptions>());
    dioWithoutJwt.interceptors.add(ErrorInterceptor());
    dioWithoutJwt.interceptors.add(LogInterceptor());
    dioWithoutJwt.interceptors.add(AppLogInterceptor());
    dio = Modular.get<DioForNative>();
  }

  @override
  void dispose() {}

  @override
  Future<AccessToken?> getCurrentToken() async {
    final dynamic hasToken =
        await LocalStorageService.getValue<String>(_currentToken);

    if (hasToken) {
      final dynamic result =
          jsonDecode(await LocalStorageService.getValue<String>(_currentToken));
      return AccessToken.fromJson(result);
    }

    return null;
  }

  @override
  Future<UserAccount> getCurrentUser() async {
    final Response<dynamic> response =
        await dio.get('/api/auth/getCurrentUser');
    return UserAccount.fromJson(response.data);
  }

  @override
  Future<bool> isAuthenticated() async =>
      LocalStorageService.cointains(_currentToken);

  @override
  Future<Either<ResponseFailure, AccessToken>> login(
    LoginInput loginInput,
  ) async {
    try {
      final Response<dynamic> response =
          await dio.post('/api/auth/login', data: loginInput.toJson());
      final AccessToken result = AccessToken.fromJson(response.data);
      return Right<ResponseFailure, AccessToken>(result);
    } on DioException catch (error) {
      return Left<ResponseFailure, AccessToken>(ResponseFailure.fromJson(
          error.response?.data, error.response?.statusCode));
    }
  }

  @override
  Future<void> logout() async {
    await removeCurrentToken();
    Modular.to.navigate('login');
  }

  @override
  Future<Either<ResponseFailure, AccessToken>> refreshToken(
    String refreshTokenModel,
  ) async {
    try {
      final Response<dynamic> response = await dioWithoutJwt.post(
          '/api/auth/refresh-token',
          data:
              jsonEncode(<String, String>{'refreshToken': refreshTokenModel}));
      final AccessToken result = AccessToken.fromJson(response.data);
      return Right<ResponseFailure, AccessToken>(result);
    } on DioException catch (err) {
      return Left<ResponseFailure, AccessToken>(ResponseFailure.fromJson(
          err.response?.data, err.response?.statusCode));
    }
  }

  @override
  Future<void> removeCurrentToken() async {
    final bool hasToken = await LocalStorageService.cointains(_currentToken);

    if (hasToken) {
      await LocalStorageService.removeValue(_currentToken);
    }
  }

  @override
  Future<void> setCurrentToken(AccessToken token) async =>
      LocalStorageService.setValue<String>(
          _currentToken, jsonEncode(token.toJson()));
}
