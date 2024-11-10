import 'package:flutter/material.dart';
import 'package:flutter_modular/flutter_modular.dart';
import 'package:merkant/home/app_module.dart';
import 'package:merkant/home/merkant.dart';
import 'package:timeago/timeago.dart' as timeago;

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();

  timeago.setLocaleMessages('en', timeago.EnMessages());
  runApp(ModularApp(module: AppModule(), child: const Merkant()));
}
