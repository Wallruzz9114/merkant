import 'package:flutter_modular/flutter_modular.dart';
import 'package:mobx/mobx.dart';

part 'home_screen_controller.g.dart';

class HomeScreenController = HomeScreenControllerBase
    with _$HomeScreenController;

abstract class HomeScreenControllerBase with Store, Disposable {
  @observable
  bool isVisibleNovos = true;

  @observable
  bool isVisibleMaisVendidos = true;

  @observable
  bool isVisibleUltimosVendidos = true;

  @observable
  bool isVisibleFavoritos = true;

  @observable
  int currentIndexCarouselSlider = 0;

  @override
  void dispose() {}
}
