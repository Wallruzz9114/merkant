import 'package:dio/dio.dart';
import 'package:dio/io.dart';
import 'package:merkant/shared/utils/http/error_interceptor.dart';
import 'package:merkant/shared/utils/http/log_interceptor.dart';

import 'bearer_interceptor.dart';
import 'dio_adapter_stub.dart'
    if (dart.library.io) 'dio_adapter_mobile.dart'
    if (dart.library.js) 'dio_adapter_web.dart';

class DioApi extends DioForNative {
  DioApi(BaseOptions options) : super(options) {
    interceptors.add(BearerInterceptor());
    interceptors.add(ErrorInterceptor());
    interceptors.add(LogInterceptor());
    interceptors.add(AppLogInterceptor());

    httpClientAdapter = getAdapter();
  }
}
