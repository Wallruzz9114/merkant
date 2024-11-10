import 'package:flutter/material.dart';
// import 'package:flutter_modular/flutter_modular.dart';

class SplashScreen extends StatefulWidget {
  const SplashScreen({Key? key, this.title = 'Splash'}) : super(key: key);

  final String title;

  @override
  State<SplashScreen> createState() => _SplashScreenState();
}

class _SplashScreenState extends State<SplashScreen> {
  @override
  void initState() {
    super.initState();

    Future<dynamic>.delayed(const Duration(seconds: 1))
        .then((dynamic value) async {
      // Modular.to.pushReplacementNamed('/tab/');
    });
  }

  @override
  Scaffold build(BuildContext context) => Scaffold(
        body: Container(
          decoration: const BoxDecoration(
            gradient: LinearGradient(
              begin: Alignment.topCenter,
              end: Alignment.bottomCenter,
              colors: <Color>[Colors.white, Color(0xFFedf2f6)],
            ),
          ),
          child: Center(
              child: Image.asset(
            'assets/appicon_512x512.png',
            fit: BoxFit.scaleDown,
          )),
        ),
      );
}
