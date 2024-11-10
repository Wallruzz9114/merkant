import 'package:dartz/dartz.dart';
import 'package:flutter_modular/flutter_modular.dart';
import 'package:merkant/models/access_token.dart';
import 'package:merkant/models/login_input.dart';
import 'package:merkant/models/response_error.dart';
import 'package:merkant/models/user_account.dart';

abstract class IAuthService implements Disposable {
  Future<UserAccount> getCurrentUser();
  Future<AccessToken?> getCurrentToken();
  Future<void> setCurrentToken(AccessToken token);
  Future<void> removeCurrentToken();
  Future<bool> isAuthenticated();
  Future<void> logout();
  Future<Either<ResponseFailure, AccessToken>> login(LoginInput loginInput);
  Future<Either<ResponseFailure, AccessToken>> refreshToken(
      String refreshTokenModel);
}
