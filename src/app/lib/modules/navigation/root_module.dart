import 'package:flutter_modular/flutter_modular.dart';
import 'package:merkant/modules/navigation/root_screen.dart';
import 'package:modular_interfaces/modular_interfaces.dart';

class RootModule extends Module {
  static const String routeName = '/root/';

  @override
  final List<Bind<Object>> binds = <Bind<Object>>[];

  @override
  final List<ModularRoute> routes = <ModularRoute>[
    ChildRoute<RootScreen>(Modular.initialRoute,
        child: (_, ModularArguments args) => const RootScreen()),
  ];
}
