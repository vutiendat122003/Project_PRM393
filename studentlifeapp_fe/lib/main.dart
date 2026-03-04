import 'package:flutter/material.dart';
import 'screens/tt01_insert_score_screen.dart';
import 'screens/tt02_get_score_screen.dart';
import 'screens/tt03_gpa_screen.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Flutter Demo',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(
          seedColor: Colors.deepPurple,
        ),
        useMaterial3: true,
      ),

 
      home: const MenuScreen(),
    );
  }
}


class MenuScreen extends StatelessWidget {
  const MenuScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Student Life App"),
      ),
      body: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [

            /// TT-01
            ElevatedButton(
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (_) =>
                        const TT01InsertScoreScreen(),
                  ),
                );
              },
              child: const Text("TT-01 Insert Score"),
            ),

            const SizedBox(height: 15),

            /// TT-02
            ElevatedButton(
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (_) =>
                        const TT02GetScoreScreen(
                          studentId: 1,
                        ),
                  ),
                );
              },
              child: const Text("TT-02 Get Scores"),
            ),

            const SizedBox(height: 15),

            /// TT-03
            ElevatedButton(
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (_) =>
                        const TT03GPAScreen(
                          studentId: 1,
                        ),
                  ),
                );
              },
              child: const Text("TT-03 GPA"),
            ),

            const SizedBox(height: 30),

       
            ElevatedButton(
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (_) =>
                        const MyHomePage(
                          title:
                          "Flutter Demo Home Page",
                        ),
                  ),
                );
              },
              child: const Text("Open Counter Demo"),
            ),
          ],
        ),
      ),
    );
  }
}



class MyHomePage extends StatefulWidget {
  const MyHomePage({super.key, required this.title});

  final String title;

  @override
  State<MyHomePage> createState() =>
      _MyHomePageState();
}

class _MyHomePageState
    extends State<MyHomePage> {

  int _counter = 0;

  void _incrementCounter() {
    setState(() {
      _counter++;
    });
  }

  @override
  Widget build(BuildContext context) {

    return Scaffold(
      appBar: AppBar(
        backgroundColor:
        Theme.of(context)
            .colorScheme
            .inversePrimary,
        title: Text(widget.title),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment:
          MainAxisAlignment.center,
          children: [
            const Text(
                'You have pushed the button this many times:'),
            Text(
              '$_counter',
              style: Theme.of(context)
                  .textTheme
                  .headlineMedium,
            ),
          ],
        ),
      ),
      floatingActionButton:
      FloatingActionButton(
        onPressed:
        _incrementCounter,
        tooltip: 'Increment',
        child:
        const Icon(Icons.add),
      ),
    );
  }
}