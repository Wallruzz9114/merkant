import 'dart:convert';

import 'package:flutter_modular/flutter_modular.dart';
import 'package:merkant/stores/theme_store.dart';

bool isNullorEmpty(String? str) => str == null || str.isEmpty;

String? tryEncode(dynamic data) {
  try {
    return json.decode(data);
  } catch (e) {
    return null;
  }
}

bool isDarkModeEnabled() {
  return Modular.get<ThemeStore>().isDarkMode;
}

String greetingMessage() {
  final int h = DateTime.now().hour;
  if (h <= 5) {
    return 'Boa noite';
  }
  if (h < 12) {
    return 'Bom dia';
  }
  if (h < 18) {
    return 'Boa tarde';
  }

  return 'Boa noite';
}

String getBaseUrl() {
  const String baseUrl = String.fromEnvironment('BASE_URL', defaultValue: '');

  if (isNullorEmpty(baseUrl)) {
    throw Exception('Defina sua BASE_URL em --dart-define.');
  }

  return baseUrl;
}
