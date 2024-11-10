import 'package:dio/browser.dart';
import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';

HttpClientAdapter getAdapter() {
  final BrowserHttpClientAdapter browserHttpClientAdapter =
      BrowserHttpClientAdapter();

  if (kDebugMode) {
    browserHttpClientAdapter.withCredentials = false;
  }

  return browserHttpClientAdapter;
}
