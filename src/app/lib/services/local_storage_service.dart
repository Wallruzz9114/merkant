import 'package:shared_preferences/shared_preferences.dart';

class LocalStorageService {
  static Future<dynamic> getValue<T>(String key) async {
    return setInstance().then((SharedPreferences sharedPreferences) {
      switch (T) {
        case double:
          return sharedPreferences.getDouble(key) ?? 0;
        case int:
          return sharedPreferences.getInt(key) ?? 0;
        case String:
          return sharedPreferences.getString(key) ?? '';
        case const (List<dynamic>):
          return sharedPreferences.getStringList(key) ?? <dynamic>[];
        case bool:
          return sharedPreferences.getBool(key) ?? false;
        default:
          return sharedPreferences.getString(key) ?? '';
      }
    });
  }

  static Future<SharedPreferences> setInstance() async {
    return SharedPreferences.getInstance();
  }

  static Future<void> removeValue(String key) async {
    await setInstance().then(
      (SharedPreferences sharedPreferences) => sharedPreferences.remove(key),
    );
  }

  static Future<bool> setValue<T>(String key, dynamic value) async {
    return setInstance().then((SharedPreferences sharedPreferences) {
      switch (T) {
        case double:
          return sharedPreferences.setDouble(key, value);
        case int:
          return sharedPreferences.setInt(key, value);
        case String:
          return sharedPreferences.setString(key, value);
        case const (List<dynamic>):
          return sharedPreferences.setStringList(key, value);
        case bool:
          return sharedPreferences.setBool(key, value);
        default:
          return sharedPreferences.setString(key, value);
      }
    });
  }

  static Future<bool> cointains(String key) async {
    return setInstance().then((SharedPreferences sharedPreferences) {
      return sharedPreferences.containsKey(key);
    });
  }
}
