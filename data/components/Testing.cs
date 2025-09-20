using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "cf678dcd523ca2fb01a69a30da61f1007aaf5baf")]
public class Testing : Component
{
	Json json = new(), jsonQuestions;
	//private Json json = null;

	[ShowInEditor]
	//[Parameter(Title = "Json File")]
	[ParameterFile(Filter = ".json")]
	private string jsonFile = "";       //в переменной хранится оносительный путь к файлу

	struct Question
	{
		public string question;
		public List<string> answers;
		public int correctAnswer;
	}
	private List<Question> questions = new List<Question>();


	private WidgetVBox VBox, VBoxFinal;
	private WidgetSprite background, backgroundFinal;
	private WidgetLabel questionLabel, numberLabel, resultLabel;
	private WidgetButton btn1, btn2, btn3, btn4, nextButton, endButton, exitButton;

	private List<WidgetButton> buttons = new List<WidgetButton>();


	int correctAnswers = 0, currentQuestionIndex = 0;
	bool isTestGoing = false;

	public Widget TestBox, TestBoxResults;


	void Init()
	{
		LoadQuestions();		//загружаем текст вопросов с ответами из файла
		CreateTestingMenu();	//создаем меню
		CreateTestingFinal();	//создаем окошко с результатами тестирования

		VBox.Hidden = true;
		VBoxFinal.Hidden = true;

		TestBox = VBox;
		TestBoxResults = VBoxFinal;
	}

	void Update()
	{
		//Log.Message("Полученное имя Json файла: {0}\n", jsonFile);
		if (Input.IsKeyDown(Input.KEY.T))		//для проверок
		{
			StartTesting();
		}
		if (isTestGoing)			//пока меню открыто, при нажатии в пустое будем отключать захват мышки (для поворота камеры) и будем включать курсор, 
		{							//так как иначе при нажатии в пустое место они исчезают
			Input.MouseGrab = false;
			Gui gui;
			gui = Gui.GetCurrent();
			gui.MouseShow = true;
		}
	}


	private void LoadQuestions()
	{
		json.Load(jsonFile);     					   //загружаем файл по пути к файлу из переменной jsonFile
		jsonQuestions = json.GetChild("questions");		//получаем объект questions из файла .json

		for (int i = 0; i < jsonQuestions.GetNumChildren(); i++)
		{
			Json jsonQuestion = jsonQuestions.GetChild(i);			//получаем дочерний объект по номеру вопроса из файла .json
			string question = jsonQuestion.Read("question");		//считываем текст из поля question
			int correctAnswer;
			jsonQuestion.Read("rightAnswer", out correctAnswer);	//считываем номер правильного ответа из поля rightAnswer

			List<string> answers = new List<string>();
			Json jsonAnswers = jsonQuestion.GetChild("answers");	//получаем дочерний объект answers из файла .json
			answers.Add(jsonAnswers.Read("1"));						//добавляем в массив строк из поля с текстом первого ответа
			answers.Add(jsonAnswers.Read("2"));
			answers.Add(jsonAnswers.Read("3"));
			answers.Add(jsonAnswers.Read("4"));

			questions.Add(new Question		//добавляем в список экземпляр структуры Question, с текстом вопроса, списком срок ответов, номером правильного ответа
			{
				question = question,
				answers = answers,
				correctAnswer = correctAnswer
			});
		}
	}

	private void CreateTestingMenu()
	{
		VBox = new WidgetVBox();
		VBox.Width = 900;
		VBox.Height = 600;

		background = new WidgetSprite();
		background.Width = 900;
		background.Height = 600;
		background.Texture = "data/gui/background_test.png";
		background.SetPosition(0, 0);

		questionLabel = new WidgetLabel();
		questionLabel.Height = 120;
		questionLabel.Width = 800;
		questionLabel.Text = "Вопрос";
		questionLabel.FontSize = 30;
		questionLabel.SetPosition(50, 50);
		questionLabel.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		questionLabel.FontWrap = 1;

		btn1 = new WidgetButton();
		btn1.Width = 380;
		btn1.Height = 140;
		btn1.Text = "Ответ 1";
		btn1.FontSize = 20;
		btn1.FontWrap = 1;
		btn1.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		btn1.SetPosition(50, 185);
		btn1.ButtonColor = new vec4(0.39f, 0.39f, 0.50f, 1.0f);
		btn1.EventClicked.Connect(CheckAnswer, 1);			//привязываем обработчик нажатия на кнопку для проверки правильности ответа

		btn2 = new WidgetButton();
		btn2.Width = 380;
		btn2.Height = 140;
		btn2.Text = "Ответ 2";
		btn2.FontSize = 20;
		btn2.FontWrap = 1;
		btn2.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		btn2.SetPosition(470, 185);
		btn2.ButtonColor = new vec4(0.39f, 0.39f, 0.50f, 1.0f);
		btn2.EventClicked.Connect(CheckAnswer, 2);


		btn3 = new WidgetButton();
		btn3.Width = 380;
		btn3.Height = 140;
		btn3.Text = "Ответ 3";
		btn3.FontSize = 20;
		btn3.FontWrap = 1;
		btn3.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		btn3.SetPosition(50, 350);
		btn3.ButtonColor = new vec4(0.39f, 0.39f, 0.50f, 1.0f);
		btn3.EventClicked.Connect(CheckAnswer, 3);

		btn4 = new WidgetButton();
		btn4.Width = 380;
		btn4.Height = 140;
		btn4.Text = "Ответ 4";
		btn4.FontSize = 20;
		btn4.FontWrap = 1;
		btn4.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		btn4.SetPosition(470, 350);
		btn4.ButtonColor = new vec4(0.39f, 0.39f, 0.50f, 1.0f);
		btn4.EventClicked.Connect(CheckAnswer, 4);

		numberLabel = new WidgetLabel();
		numberLabel.Width = 70;
		numberLabel.Height = 40;
		numberLabel.Text = "1/5";
		numberLabel.FontSize = 30;
		numberLabel.SetPosition(415, 527);
		numberLabel.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);

		nextButton = new WidgetButton();
		nextButton.Width = 220;
		nextButton.Height = 70;
		nextButton.Text = "Далее";
		nextButton.FontSize = 25;
		nextButton.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		nextButton.SetPosition(630, 511);
		nextButton.ButtonColor = new vec4(0.39f, 0.39f, 0.50f, 1.0f);
		nextButton.EventClicked.Connect(ClickNextButton);

		exitButton = new WidgetButton();
		exitButton.Width = 220;
		exitButton.Height = 70;
		exitButton.Text = "Закрыть";
		exitButton.FontSize = 25;
		exitButton.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		exitButton.SetPosition(50, 511);
		exitButton.ButtonColor = new vec4(0.39f, 0.39f, 0.50f, 1.0f);
		exitButton.EventClicked.Connect(ClickExitButton);


		btn3 = new WidgetButton();
		btn3.Width = 380;
		btn3.Height = 140;
		btn3.Text = "Ответ 3";
		btn3.FontSize = 20;
		btn3.FontWrap = 1;
		btn3.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		btn3.SetPosition(50, 350);
		btn3.ButtonColor = new vec4(0.39f, 0.39f, 0.50f, 1.0f);
		btn3.EventClicked.Connect(CheckAnswer, 3);

		btn4 = new WidgetButton();
		btn4.Width = 380;
		btn4.Height = 140;
		btn4.Text = "Ответ 4";
		btn4.FontSize = 20;
		btn4.FontWrap = 1;
		btn4.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		btn4.SetPosition(470, 350);
		btn4.ButtonColor = new vec4(0.39f, 0.39f, 0.50f, 1.0f);
		btn4.EventClicked.Connect(CheckAnswer, 4);

		numberLabel = new WidgetLabel();
		numberLabel.Width = 70;
		numberLabel.Height = 40;
		numberLabel.Text = "1/5";
		numberLabel.FontSize = 30;
		numberLabel.SetPosition(415, 527);
		numberLabel.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);

		nextButton = new WidgetButton();
		nextButton.Width = 220;
		nextButton.Height = 70;
		nextButton.Text = "Далее";
		nextButton.FontSize = 25;
		nextButton.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		nextButton.SetPosition(630, 511);
		nextButton.ButtonColor = new vec4(0.39f, 0.39f, 0.50f, 1.0f);
		nextButton.EventClicked.Connect(ClickNextButton);

		exitButton = new WidgetButton();
		exitButton.Width = 220;
		exitButton.Height = 70;
		exitButton.Text = "Закрыть";
		exitButton.FontSize = 25;
		exitButton.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		exitButton.SetPosition(50, 511);
		exitButton.ButtonColor = new vec4(0.39f, 0.39f, 0.50f, 1.0f);
		exitButton.EventClicked.Connect(ClickExitButton);


		VBox.AddChild(background, Gui.ALIGN_OVERLAP);
		VBox.AddChild(questionLabel, Gui.ALIGN_OVERLAP);
		VBox.AddChild(btn1, Gui.ALIGN_OVERLAP);
		VBox.AddChild(btn2, Gui.ALIGN_OVERLAP);
		VBox.AddChild(btn3, Gui.ALIGN_OVERLAP);
		VBox.AddChild(btn4, Gui.ALIGN_OVERLAP);
		VBox.AddChild(numberLabel, Gui.ALIGN_OVERLAP);
		VBox.AddChild(nextButton, Gui.ALIGN_OVERLAP);
		VBox.AddChild(exitButton, Gui.ALIGN_OVERLAP);

		WindowManager.MainWindow.AddChild(VBox, Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER);

		buttons.Add(btn1);		//добавляем в список кнопку
		buttons.Add(btn2);
		buttons.Add(btn3);
		buttons.Add(btn4);

		nextButton.Enabled = false;
	}


	private void CreateTestingFinal()
	{
		VBoxFinal = new WidgetVBox();
		VBoxFinal.Width = 450;
		VBoxFinal.Height = 250;

		backgroundFinal = new WidgetSprite();
		backgroundFinal.Width = 450;
		backgroundFinal.Height = 250;
		backgroundFinal.Texture = "data/gui/background_test_results.png";
		backgroundFinal.SetPosition(0, 0);

		resultLabel = new WidgetLabel();
		resultLabel.Height = 120;
		resultLabel.Width = 350;
		resultLabel.Text = "Ваш результат: ";
		resultLabel.FontSize = 25;
		resultLabel.SetPosition(50, 50);
		resultLabel.TextAlign = Gui.ALIGN_CENTER;
		resultLabel.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		resultLabel.FontWrap = 1;

		endButton = new WidgetButton();
		endButton.Width = 220;
		endButton.Height = 70;
		endButton.Text = "Закрыть";
		endButton.FontSize = 25;
		endButton.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		endButton.SetPosition(115, 130);
		endButton.ButtonColor = new vec4(0.39f, 0.39f, 0.50f, 1.0f);
		endButton.EventClicked.Connect(ClickEndButton);

		VBoxFinal.AddChild(backgroundFinal, Gui.ALIGN_OVERLAP);
		VBoxFinal.AddChild(resultLabel, Gui.ALIGN_OVERLAP);
		VBoxFinal.AddChild(endButton, Gui.ALIGN_OVERLAP);

		WindowManager.MainWindow.AddChild(VBoxFinal, Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER);
	}


	public void StartTesting()
	{
		VBox.Hidden = false;
		correctAnswers = 0;
		currentQuestionIndex = 0;
		SetBaseButton();
		ShowQuestion();
		nextButton.Hidden = false;

		isTestGoing = true;
		EngineWindowViewport ewv = WindowManager.MainWindow;
		ewv.EventResized.Connect(ChangeMenuPosition);			//привязываем обработчик изменения размеров окна приложения
	}


	private void SetBaseButton()
	{
		foreach (var button in buttons)		//включаем отображение для каждой кнопки, также задаем им цвет
		{
			button.Enabled = true;
			button.ButtonColor = new vec4(0.39f, 0.39f, 0.50f, 1.0f);
		}
	}


	private void ShowQuestion()
	{
		Question question = questions[currentQuestionIndex];	//создаем структуру Question и добавляем в нее currentQuestionIndex'овый вопрос из списка вопросов
		questionLabel.Text = question.question;					//записываем в текстовое поле текст текущего вопроса
		btn1.Text = question.answers[0];						//записываем на кнопку текст первого ответа из currentQuestionIndex вопроса
		btn2.Text = question.answers[1];
		btn3.Text = question.answers[2];
		btn4.Text = question.answers[3];
		numberLabel.Text = (currentQuestionIndex + 1) + "/" + questions.Count;	//записываем в текстовое поле номер текущего вопроса и суммарное количесво вопросов
	}


	private void CheckAnswer(int answer)
	{
		if (answer == questions[currentQuestionIndex].correctAnswer)
		{
			ShowAnswer(true, answer);		//отображаем что выбранный ответ правильный
			correctAnswers++;				//прибавляем счетчик правильных ответов
		}
		else
		{
			ShowAnswer(false, answer);		//отображаем что выбранный ответ неправильный
		}

		foreach (var button in buttons)		//отключаем доступность всех кнопок
		{
			button.Enabled = false;
		}

		if (currentQuestionIndex <= questions.Count)	//если текущий индекс вопроса не превышае количество вопросов, то включаем кнопку для перехода к следующему вопросу
		{
			nextButton.Enabled = true;
		}
	}


	private void ShowAnswer(bool isRight, int answer)
	{
		switch (questions[currentQuestionIndex].correctAnswer)	//закрашиваем зеленым цветом кнопку с правильным ответом текущего вопроса
		{
			case 1:
				btn1.ButtonColor = new vec4(0, 1, 0, 1);
				break;
			case 2:
				btn2.ButtonColor = new vec4(0, 1, 0, 1);
				break;
			case 3:
				btn3.ButtonColor = new vec4(0, 1, 0, 1);
				break;
			case 4:
				btn4.ButtonColor = new vec4(0, 1, 0, 1);
				break;
		}

		if (!isRight)		//если выбранный ответ был неправильным, то закрашиваем красным цветом кнопку с выбранным ответом текущего вопроса
		{
			switch (answer)
			{
				case 1:
					btn1.ButtonColor = new vec4(1, 0, 0, 1);
					break;
				case 2:
					btn2.ButtonColor = new vec4(1, 0, 0, 1);
					break;
				case 3:
					btn3.ButtonColor = new vec4(1, 0, 0, 1);
					break;
				case 4:
					btn4.ButtonColor = new vec4(1, 0, 0, 1);
					break;
			}
		}
	}


	private void ClickNextButton()
	{
		if (currentQuestionIndex == questions.Count - 2)
		{
			nextButton.Text = "Завершить";
		}

		if (currentQuestionIndex == questions.Count - 1)
		{
			VBoxFinal.Hidden = false;
			VBox.Hidden = true;
			resultLabel.Text = "Ваш результат: " + correctAnswers + "/" + questions.Count;
		}
		else
		{
			nextButton.Enabled = false;
			currentQuestionIndex++;
			ShowQuestion();
			SetBaseButton();
		}
	}


	private void ClickExitButton()
	{
		VBox.Hidden = true;
		HideUI();					//отключаем захват мыши и отвязываем обработчик
	}

	private void ClickEndButton()
	{
		VBoxFinal.Hidden = true;
		HideUI();					//отключаем захват мыши и отвязываем обработчик
	}



	public bool getActiveTest()
	{
		return isTestGoing;
	}

	private void ChangeMenuPosition()		//изменяем размеры окна теста и результатов теста
	{
		var mainWindowSize = WindowManager.MainWindow.Size;
		var px = TestBox.Width;
		var py = TestBox.Height;
		TestBox.SetPosition(mainWindowSize.x / 2 - px / 2, mainWindowSize.y / 2 - py / 2);

		px = TestBoxResults.Width;
		py = TestBoxResults.Height;
		TestBoxResults.SetPosition(mainWindowSize.x / 2 - px / 2, mainWindowSize.y / 2 - py / 2);
	}

	private void HideUI()
	{
		isTestGoing = false;

		Input.MouseGrab = true;
		Gui gui;
		gui = Gui.GetCurrent();
		gui.MouseShow = false;

		EngineWindowViewport ewv = WindowManager.MainWindow;
		ewv.EventResized.Disconnect(ChangeMenuPosition);			//отвязываем обработчик изменения размеров окна приложения
	}
}