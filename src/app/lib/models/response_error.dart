import 'package:merkant/shared/utils/utils.dart';

class ResponseError {
  String? code;
  String? property;
  String message;

  ResponseError({
    this.code,
    this.property,
    required this.message,
  });

  ResponseError.fromJson(Map<String, dynamic> json)
      : code = json['code'],
        property = json['property'],
        message = json['message'];
}

class ResponseFailure {
  int? statusCode;
  bool isValid;
  bool hasError;
  List<ResponseError> errors;

  ResponseFailure({
    this.statusCode,
    required this.isValid,
    required this.hasError,
    required this.errors,
  });

  factory ResponseFailure.fromJson(
          Map<String, dynamic>? json, int? statusCode) =>
      ResponseFailure(
        statusCode: statusCode,
        isValid: json != null ? json['isValid'] : false,
        hasError: json != null ? json['hasError'] : false,
        errors: json != null && json['errors'] != null
            ? List<ResponseError>.from(
                json['errors']
                    .map((dynamic model) => ResponseError.fromJson(model)),
              )
            : <ResponseError>[],
      );

  String getErrorNotProperty() => errors
      .where((ResponseError e) => isNullorEmpty(e.property))
      .map((ResponseError e) => e.message)
      .join('\n');
  String getErrorByProperty(String propertyName) => errors
      .where((ResponseError e) =>
          (e.property ?? '').toLowerCase() == propertyName.toLowerCase())
      .map((ResponseError e) => e.message)
      .join('\n');
}
