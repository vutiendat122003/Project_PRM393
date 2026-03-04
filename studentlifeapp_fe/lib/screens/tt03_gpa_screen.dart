import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;

class TT03GPAScreen extends StatefulWidget {

  final int studentId;

  const TT03GPAScreen({
    super.key,
    required this.studentId,
  });

  @override
  State<TT03GPAScreen> createState() =>
      _TT03GPAScreenState();
}

class _TT03GPAScreenState
    extends State<TT03GPAScreen> {

  double gpa = 0;

  bool isLoading = true;

  Future<void> fetchGPA() async {

    final url = Uri.parse(
        "http://10.0.2.2:5000/api/score/gpa/${widget.studentId}");

    try {

      final response =
      await http.get(url);

      if (response.statusCode == 200) {

        setState(() {

          gpa =
          double.parse(
              response.body);

          isLoading = false;

        });

      }

    } catch (e) {

      print(e);

    }

  }

  @override
  void initState() {
    super.initState();
    fetchGPA();
  }

  @override
  Widget build(BuildContext context) {

    return Scaffold(
      appBar:
      AppBar(title:
      const Text("TT-03 GPA")),

      body: Center(
        child: isLoading
            ? const CircularProgressIndicator()
            : Text(
          "GPA: ${gpa.toStringAsFixed(2)}",

          style:
          const TextStyle(
            fontSize: 40,
            fontWeight:
            FontWeight.bold,
          ),
        ),
      ),
    );
  }
}