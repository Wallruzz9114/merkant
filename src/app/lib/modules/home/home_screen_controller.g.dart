// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'home_screen_controller.dart';

// **************************************************************************
// StoreGenerator
// **************************************************************************

// ignore_for_file: non_constant_identifier_names, unnecessary_brace_in_string_interps, unnecessary_lambdas, prefer_expression_function_bodies, lines_longer_than_80_chars, avoid_as, avoid_annotating_with_dynamic, no_leading_underscores_for_local_identifiers

mixin _$HomeScreenController on HomeScreenControllerBase, Store {
  late final _$isVisibleNovosAtom =
      Atom(name: 'HomeScreenControllerBase.isVisibleNovos', context: context);

  @override
  bool get isVisibleNovos {
    _$isVisibleNovosAtom.reportRead();
    return super.isVisibleNovos;
  }

  @override
  set isVisibleNovos(bool value) {
    _$isVisibleNovosAtom.reportWrite(value, super.isVisibleNovos, () {
      super.isVisibleNovos = value;
    });
  }

  late final _$isVisibleMaisVendidosAtom = Atom(
      name: 'HomeScreenControllerBase.isVisibleMaisVendidos', context: context);

  @override
  bool get isVisibleMaisVendidos {
    _$isVisibleMaisVendidosAtom.reportRead();
    return super.isVisibleMaisVendidos;
  }

  @override
  set isVisibleMaisVendidos(bool value) {
    _$isVisibleMaisVendidosAtom.reportWrite(value, super.isVisibleMaisVendidos,
        () {
      super.isVisibleMaisVendidos = value;
    });
  }

  late final _$isVisibleUltimosVendidosAtom = Atom(
      name: 'HomeScreenControllerBase.isVisibleUltimosVendidos',
      context: context);

  @override
  bool get isVisibleUltimosVendidos {
    _$isVisibleUltimosVendidosAtom.reportRead();
    return super.isVisibleUltimosVendidos;
  }

  @override
  set isVisibleUltimosVendidos(bool value) {
    _$isVisibleUltimosVendidosAtom
        .reportWrite(value, super.isVisibleUltimosVendidos, () {
      super.isVisibleUltimosVendidos = value;
    });
  }

  late final _$isVisibleFavoritosAtom = Atom(
      name: 'HomeScreenControllerBase.isVisibleFavoritos', context: context);

  @override
  bool get isVisibleFavoritos {
    _$isVisibleFavoritosAtom.reportRead();
    return super.isVisibleFavoritos;
  }

  @override
  set isVisibleFavoritos(bool value) {
    _$isVisibleFavoritosAtom.reportWrite(value, super.isVisibleFavoritos, () {
      super.isVisibleFavoritos = value;
    });
  }

  late final _$currentIndexCarouselSliderAtom = Atom(
      name: 'HomeScreenControllerBase.currentIndexCarouselSlider',
      context: context);

  @override
  int get currentIndexCarouselSlider {
    _$currentIndexCarouselSliderAtom.reportRead();
    return super.currentIndexCarouselSlider;
  }

  @override
  set currentIndexCarouselSlider(int value) {
    _$currentIndexCarouselSliderAtom
        .reportWrite(value, super.currentIndexCarouselSlider, () {
      super.currentIndexCarouselSlider = value;
    });
  }

  @override
  String toString() {
    return '''
isVisibleNovos: ${isVisibleNovos},
isVisibleMaisVendidos: ${isVisibleMaisVendidos},
isVisibleUltimosVendidos: ${isVisibleUltimosVendidos},
isVisibleFavoritos: ${isVisibleFavoritos},
currentIndexCarouselSlider: ${currentIndexCarouselSlider}
    ''';
  }
}
