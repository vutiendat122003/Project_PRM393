import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;

class TT02GetScoreScreen extends StatefulWidget {

  final int studentId;

  const TT02GetScoreScreen({
    super.key,
    required this.studentId,
  });

  @override
  State<TT02GetScoreScreen> createState() =>
      _TT02GetScoreScreenState();
}

class _TT02GetScoreScreenState
    extends State<TT02GetScoreScreen> {

  List scores = [];

  bool isLoading = true;

  Future<void> fetchScores() async {

    final url = Uri.parse(
        "http://10.0.2.2:5000/api/score/${widget.studentId}");

    try {

      final response = await http.get(url);

      if (response.statusCode == 200) {

        setState(() {
          scores =
          jsonDecode(response.body);
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
    fetchScores();
  }

  @override
  Widget build(BuildContext context) {

    return Scaffold(
      appBar: AppBar(
        title:
        const Text("TT-02 Student Scores"),
      ),

      body: isLoading
          ? const Center(
          child:
          CircularProgressIndicator())
          : ListView.builder(
        itemCount: scores.length,

        itemBuilder:
            (context, index) {

          final score =
          scores[index];

          return Card(
            child: ListTile(

              title: Text(
                  score["subject"]
                      .toString()),

              subtitle: Text(
                  "Score: ${score["score"]} | Credits: ${score["credits"]}"),

            ),
          );
        },
      ),
    );
  }
}