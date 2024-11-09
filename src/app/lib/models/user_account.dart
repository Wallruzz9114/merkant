import 'package:merkant/shared/utils/utils.dart';

class UserAccount {
  UserAccount({
    this.id,
    required this.nome,
    required this.username,
    required this.email,
    required this.phoneNumber,
    this.avatarUrl,
    required this.isActive,
    required this.roles,
  });

  late final String? id;
  late final String nome;
  late final String username;
  late final String email;
  late final String phoneNumber;
  late final String? avatarUrl;
  late final bool isActive;
  late final List<String> roles;

  UserAccount.fromJson(Map<String, dynamic> json) {
    id = json['id'];
    nome = json['nome'];
    username = json['username'];
    email = json['email'];
    phoneNumber = json['phoneNumber'];
    avatarUrl = json['avatarUrl'];
    isActive = json['isActive'];
    roles = List.castFrom<dynamic, String>(json['roles']);
  }

  bool get isEditing => !isNullorEmpty(id);
  bool get isAdmin =>
      roles.any((String element) => element.toLowerCase() == 'admin');
}
