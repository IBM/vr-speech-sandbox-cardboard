﻿pandoc ../README.md -f markdown -t html -s -o ../README.html
pandoc ../Scripts/Services/AlchemyAPI/v1/README.md -f markdown -t html -s -o ../Scripts/Services/AlchemyAPI/v1/index.html
pandoc ../Scripts/Services/Assistant/v1/README.md -f markdown -t html -s -o ../Scripts/Services/Assistant/v1/index.html
pandoc ../Scripts/Services/Conversation/v1/README.md -f markdown -t html -s -o ../Scripts/Services/Conversation/v1/index.html
pandoc ../Scripts/Services/Discovery/v1/README.md -f markdown -t html -s -o ../Scripts/Services/Discovery/v1/index.html
pandoc ../Scripts/Services/DocumentConversion/v1/README.md -f markdown -t html -s -o ../Scripts/Services/DocumentConversion/v1/index.html
pandoc ../Scripts/Services/LanguageTranslator/v2/README.md -f markdown -t html -s -o ../Scripts/Services/LanguageTranslator/v2/index.html
pandoc ../Scripts/Services/NaturalLanguageClassifier/v2/README.md -f markdown -t html -s -o ../Scripts/Services/NaturalLanguageClassifier/v2/index.html
pandoc ../Scripts/Services/NaturalLanguageUnderstanding/v1/README.md -f markdown -t html -s -o ../Scripts/Services/NaturalLanguageUnderstanding/v1/index.html
pandoc ../Scripts/Services/PersonalityInsights/v3/README.md -f markdown -t html -s -o ../Scripts/Services/PersonalityInsights/v3/index.html
pandoc ../Scripts/Services/RetrieveAndRank/v1/README.md -f markdown -t html -s -o ../Scripts/Services/RetrieveAndRank/v1/index.html
pandoc ../Scripts/Services/SpeechToText/v1/README.md -f markdown -t html -s -o ../Scripts/Services/SpeechToText/v1/index.html
pandoc ../Scripts/Services/TextToSpeech/v1/README.md -f markdown -t html -s -o ../Scripts/Services/TextToSpeech/v1/index.html
pandoc ../Scripts/Services/ToneAnalyzer/v3/README.md -f markdown -t html -s -o ../Scripts/Services/ToneAnalyzer/v3/index.html
pandoc ../Scripts/Services/TradeoffAnalytics/v1/README.md -f markdown -t html -s -o ../Scripts/Services/TradeoffAnalytics/v1/index.html
pandoc ../Scripts/Services/VisualRecognition/v3/README.md -f markdown -t html -s -o ../Scripts/Services/VisualRecognition/v3/index.html

Remove-Item ../README.md
Remove-Item ../Scripts/Services/AlchemyAPI/v1/README.md
Remove-Item ../Scripts/Services/Assistant/v1/README.md
Remove-Item ../Scripts/Services/Conversation/v1/README.md
Remove-Item ../Scripts/Services/Discovery/v1/README.md
Remove-Item ../Scripts/Services/DocumentConversion/v1/README.md
Remove-Item ../Scripts/Services/LanguageTranslator/v2/README.md
Remove-Item ../Scripts/Services/NaturalLanguageClassifier/v2/README.md
Remove-Item ../Scripts/Services/NaturalLanguageUnderstanding/v1/README.md
Remove-Item ../Scripts/Services/PersonalityInsights/v3/README.md
Remove-Item ../Scripts/Services/RetrieveAndRank/v1/README.md
Remove-Item ../Scripts/Services/SpeechToText/v1/README.md
Remove-Item ../Scripts/Services/TextToSpeech/v1/README.md
Remove-Item ../Scripts/Services/ToneAnalyzer/v3/README.md
Remove-Item ../Scripts/Services/TradeoffAnalytics/v1/README.md
Remove-Item ../Scripts/Services/VisualRecognition/v3/README.md
