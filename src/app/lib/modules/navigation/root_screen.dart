import 'package:animated_bottom_navigation_bar/animated_bottom_navigation_bar.dart';
import 'package:flutter/material.dart';
import 'package:flutter_modular/flutter_modular.dart';
import 'package:material_design_icons_flutter/material_design_icons_flutter.dart';
import 'package:merkant/modules/home/home_screen.dart';

class RootScreen extends StatefulWidget {
  const RootScreen({Key? key, this.title = 'Home'}) : super(key: key);

  final String title;

  @override
  State<RootScreen> createState() => _RootScreenState();
}

class _RootScreenState extends State<RootScreen> {
  final List<IconData> _iconDataList = <IconData>[
    MdiIcons.homeOutline,
    MdiIcons.homeOutline,
  ];

  List<Widget> _screens() => <Widget>[const HomeScreen(), const HomeScreen()];

  int _bottomNavIndex = 0;

  @override
  PopScope build(BuildContext context) => PopScope(
        canPop: false,
        child: Scaffold(
          resizeToAvoidBottomInset: false,
          bottomNavigationBar: AnimatedBottomNavigationBar.builder(
            itemCount: _iconDataList.length,
            tabBuilder: (int index, bool isActive) {
              return Column(
                mainAxisSize: MainAxisSize.min,
                mainAxisAlignment: MainAxisAlignment.center,
                children: <Widget>[
                  IconButton(
                    onPressed: () async {
                      setState(() => _bottomNavIndex = index);
                    },
                    icon: Icon(
                      _iconDataList[index],
                      color: Theme.of(context)
                          .bottomNavigationBarTheme
                          .unselectedIconTheme!
                          .color,
                      size: Theme.of(context)
                          .bottomNavigationBarTheme
                          .unselectedIconTheme!
                          .size,
                    ),
                  ),
                  const SizedBox(height: 5),
                  if (isActive)
                    Container(
                      height: 2,
                      width: 15,
                      color: Theme.of(context).primaryColorLight,
                    )
                  else
                    const SizedBox()
                ],
              );
            },
            activeIndex: _bottomNavIndex,
            splashRadius: 0,
            elevation: 0,
            backgroundColor:
                Theme.of(context).bottomNavigationBarTheme.backgroundColor,
            notchSmoothness: NotchSmoothness.softEdge,
            gapLocation: GapLocation.center,
            onTap: (int index) {},
          ),
          floatingActionButtonLocation:
              FloatingActionButtonLocation.centerDocked,
          floatingActionButton: FloatingActionButton(
            elevation: 0,
            backgroundColor: const Color(0xFFFA692C),
            onPressed: () {
              Modular.to.pushNamed('/cart/');
            },
            child: Icon(
              MdiIcons.shopping,
              color: Colors.white,
              size: Theme.of(context)
                  .bottomNavigationBarTheme
                  .unselectedIconTheme!
                  .size,
            ),
          ),
          body: _screens().elementAt(_bottomNavIndex),
        ),
      );
}
