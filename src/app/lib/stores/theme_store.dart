import 'package:merkant/services/local_storage_service.dart';
import 'package:mobx/mobx.dart';

part 'theme_store.g.dart';

class ThemeStore = ThemeStoreBase with _$ThemeStore;

abstract class ThemeStoreBase with Store {
  static const String _isDarkMode = '_isDarkMode';
  bool isDarkByDefault = true;

  ThemeStoreBase() {
    _initStore();
  }

  Future<void> _initStore() async {
    isDarkByDefault = await hasDarkModeSet();
  }

  @observable
  bool isDarkMode = true;

  @action
  Future<void> setDarkMode(bool isDarkMode) async {}

  Future<bool> hasDarkModeSet() async {
    final bool contains = await LocalStorageService.cointains(_isDarkMode);

    if (contains) {
      return await LocalStorageService.getValue<bool>(_isDarkMode);
    } else {
      return isDarkByDefault;
    }
  }
}
