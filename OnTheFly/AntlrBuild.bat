:: NOTE:
:: Make sure you installed Antlr4 correctly, and added the executable to your enviroment PATH.

antlr4 -Dlanguage=CSharp -package FlyLang -o Parser -visitor Fly.g4