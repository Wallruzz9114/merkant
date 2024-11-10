import 'package:flutter_modular/flutter_modular.dart';
import 'package:merkant/modules/splash/splash_module.dart';
import 'package:modular_interfaces/modular_interfaces.dart';

class SplashModule extends Module {
  @override
  final List<Bind<Object>> binds = <Bind<Object>>[];

  @override
  final List<ModularRoute> routes = <ModularRoute>[
    ChildRoute<SplashScreen>(
      Modular.initialRoute,
      child: (_, ModularArguments args) => const SplashScreen(),
    ),
  ];
}
