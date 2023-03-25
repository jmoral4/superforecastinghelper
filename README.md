# superforecastinghelper
A .NET cross platform console tool for recording predictions and calculating Brier Scores. Predictions and Score - Stored in local Sqlitedb (flat file).

## Brier Score
The Brier score, named after Glenn W. Brier, is a metric used to evaluate the accuracy of probabilistic predictions. It is commonly used in the context of forecasting, including in the evaluation of superforecasters.

The Brier score is calculated as follows:

Brier score = (1/N) * Σ(Pi - Oi)^2

where:
* N is the total number of predictions
* Pi is the predicted probability of event i
* Oi is the actual outcome of event i (1 if the event occurred, 0 if it didn't)

The Brier score ranges from 0 to 1, with lower values indicating more accurate predictions.


## Superforecasting and GJP
[Good Judment Open](https://www.gjopen.com/challenges/71-in-the-news-2023)

The Good Judgment Project (GJP) is a research initiative that focuses on the study and improvement of forecasting and decision-making abilities. It was launched in 2011 by psychologists and decision scientists Philip Tetlock, Barbara Mellers, and other colleagues. The project gained prominence as part of the Intelligence Advanced Research Projects Activity's (IARPA) Aggregative Contingent Estimation (ACE) program, which aimed to enhance the accuracy of intelligence forecasts.

The Good Judgment Project relied on the "wisdom of the crowd" principle, utilizing a large and diverse pool of participants (called "superforecasters") to make predictions about various geopolitical, economic, and other events. These individuals were selected through a series of forecasting tournaments, where they demonstrated exceptional accuracy and consistency in their predictions.

The GJP discovered that by aggregating the forecasts of these superforecasters, they could produce more accurate predictions than traditional forecasting methods, such as expert opinion or complex algorithms. Key findings from the project include:

Training and feedback: Providing participants with basic training in probabilistic reasoning and offering immediate feedback on their forecasting accuracy helped improve their predictions.

Team collaboration: Forecasters who worked in teams tended to perform better than those working individually, as they could share information, debate, and refine their forecasts.

Aggregating forecasts: Combining individual forecasts, especially those of top performers, improved overall accuracy.

Cognitive diversity: A diverse group of forecasters with different perspectives, expertise, and cognitive styles contributed to better predictions.

The Good Judgment Project has significantly impacted the fields of decision science and forecasting. It has inspired the creation of the Good Judgment Inc., a commercial spin-off that offers forecasting services for businesses and governments. The findings from the project have also been popularized in Philip Tetlock and Dan M. Gardner's book, "Superforecasting: The Art and Science of Prediction," which delves into the methods and mindset of successful forecasters.
