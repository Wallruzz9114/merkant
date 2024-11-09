import 'package:flutter/material.dart';
import 'package:flutter_mobx/flutter_mobx.dart';
import 'package:flutter_modular/flutter_modular.dart';
import 'package:merkant/shared/theme/app_theme.dart';
import 'package:merkant/stores/theme_store.dart';

class Merkant extends StatelessWidget {
  static GlobalKey<NavigatorState> navigatorKey = GlobalKey<NavigatorState>();

  const Merkant({super.key});

  @override
  Observer build(BuildContext context) {
    Modular.setNavigatorKey(navigatorKey);

    return Observer(
      builder: (BuildContext context) => MaterialApp.router(
        debugShowCheckedModeBanner: false,
        title: 'Merkant',
        theme: nativeTheme(
          isDarkModeEnable: Modular.get<ThemeStore>().isDarkMode,
        ),
        routerDelegate: Modular.routerDelegate,
        routeInformationParser: Modular.routeInformationParser,
      ),
    );
  }
}
