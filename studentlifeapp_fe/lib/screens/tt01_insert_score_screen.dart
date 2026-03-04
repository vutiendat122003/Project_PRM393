import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;

class TT01InsertScoreScreen extends StatefulWidget {
  const TT01InsertScoreScreen({super.key});

  @override
  State<TT01InsertScoreScreen> createState() =>
      _TT01InsertScoreScreenState();
}

class _TT01InsertScoreScreenState
    extends State<TT01InsertScoreScreen> {

  final studentIdController = TextEditingController();
  final assignmentIdController = TextEditingController();
  final scoreController = TextEditingController();

  String message = "";

  Future<void> insertScore() async {

    final url =
        Uri.parse("http://10.0.2.2:5000/api/score");

    final body = {
      "studentID":
          int.parse(studentIdController.text),
      "assignmentID":
          int.parse(assignmentIdController.text),
      "score":
          double.parse(scoreController.text)
    };

    try {

      final response = await http.post(
        url,
        headers: {
          "Content-Type": "application/json"
        },
        body: jsonEncode(body),
      );

      if (response.statusCode == 200) {
        setState(() {
          message = "Insert success!";
        });
      } else {
        setState(() {
          message = "Insert failed!";
        });
      }

    } catch (e) {

      setState(() {
        message = "Error: $e";
      });

    }
  }

  @override
  Widget build(BuildContext context) {

    return Scaffold(
      appBar: AppBar(
        title:
        const Text("TT-01 Teacher Insert Score"),
      ),

      body: Padding(
        padding: const EdgeInsets.all(20),

        child: Column(
          children: [

            TextField(
              controller: studentIdController,
              decoration:
              const InputDecoration(
                  labelText: "Student ID"),
            ),

            TextField(
              controller: assignmentIdController,
              decoration:
              const InputDecoration(
                  labelText: "Assignment ID"),
            ),

            TextField(
              controller: scoreController,
              decoration:
              const InputDecoration(
                  labelText: "Score"),
            ),

            const SizedBox(height: 20),

            ElevatedButton(
              onPressed: insertScore,
              child:
              const Text("Insert Score"),
            ),

            const SizedBox(height: 20),

            Text(
              message,
              style: const TextStyle(
                  fontSize: 18),
            ),

          ],
        ),
      ),
    );
  }
}