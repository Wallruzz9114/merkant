import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_modular/flutter_modular.dart';
import 'package:merkant/modules/navigation/root_module.dart';
import 'package:merkant/modules/splash/splash_screen.dart';
import 'package:merkant/shared/utils/http/dio_api.dart';
import 'package:merkant/shared/utils/utils.dart';
import 'package:merkant/stores/theme_store.dart';
import 'package:modular_interfaces/modular_interfaces.dart';

class AppModule extends Module {
  @override
  final List<Bind<Object>> binds = <Bind<Object>>[
    Bind.lazySingleton((Injector<dynamic> i) => DioApi(i.get<BaseOptions>())),
    Bind<Object>(
      (Injector<dynamic> i) => BaseOptions(
        baseUrl: getBaseUrl(),
        connectTimeout:
            const Duration(milliseconds: kReleaseMode ? 20000 : 60000),
        receiveTimeout:
            const Duration(milliseconds: kReleaseMode ? 20000 : 60000),
        sendTimeout: const Duration(milliseconds: kReleaseMode ? 20000 : 60000),
      ),
    ),
    Bind.singleton((Injector<dynamic> i) => ThemeStore()),
  ];

  @override
  final List<ModularRoute> routes = <ModularRoute>[
    ModuleRoute<SplashModule>(Modular.initialRoute, module: SplashModule()),
    ModuleRoute<RootModule>(RootModule.routeName, module: RootModule())
  ];
}
