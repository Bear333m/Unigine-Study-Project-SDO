using System.Collections;
using System.Collections.Generic;
using Unigine;
//using System.Threading.Tasks;

[Component(PropertyGuid = "584c0949b02f968405468db0830ca8483a26da28")]
public class MenuUI : Component
{
	[ShowInEditor]
	[ParameterFile(Filter = ".ui")]
	private string filePath = "null";

	[ShowInEditor]
	private Testing test;

	private UserInterface ui;
	public Widget pMainMenu;
	private bool isMenuOpened = true;
	private WidgetSprite pwButtonExit, pwButtonDisk, pwButtonWork, pwButtonTesting;


	void Init()
	{
		isMenuOpened = true;
		ui = new UserInterface(Gui.GetCurrent(), filePath);		//получаем интерфейс из пути к файлу, хранящегося в переменной filePath
		ui.Lifetime = Widget.LIFETIME.WORLD;
		System.Console.WriteLine(ui.FindWidget("mainMenu"));
		pMainMenu = ui.GetWidget(ui.FindWidget("mainMenu"));	//получаем виджет из этого файла с названием mainMenu
		pMainMenu.Lifetime = Widget.LIFETIME.WORLD;

		var mainWindowSize = WindowManager.MainWindow.Size;
		var px = pMainMenu.Width;
		var py = pMainMenu.Height;

		pwButtonExit = (WidgetSprite)ui.GetWidget(ui.FindWidget("exit"));
		pwButtonExit.EventEnter.Connect(ChangeCoverButtonEnter, pwButtonExit);		//привязываем обработчик при входе курсора на кнопку
		pwButtonExit.EventLeave.Connect(ChangeCoverButtonLeave, pwButtonExit);		//привязываем обработчик при выходе курсора на кнопку
		pwButtonExit.EventClicked.Connect(Exit);									//привязываем обработчик при нажатии на кнопку

		pwButtonDisk = (WidgetSprite)ui.GetWidget(ui.FindWidget("diskWorld"));
		pwButtonDisk.EventEnter.Connect(ChangeCoverButtonEnter, pwButtonDisk);
		pwButtonDisk.EventLeave.Connect(ChangeCoverButtonLeave, pwButtonDisk);
		pwButtonDisk.EventClicked.Connect(Disk);

		pwButtonWork = (WidgetSprite)ui.GetWidget(ui.FindWidget("workWorld"));
		pwButtonWork.EventEnter.Connect(ChangeCoverButtonEnter, pwButtonWork);
		pwButtonWork.EventLeave.Connect(ChangeCoverButtonLeave, pwButtonWork);
		pwButtonWork.EventClicked.Connect(Work);

		pwButtonTesting = (WidgetSprite)ui.GetWidget(ui.FindWidget("testing"));
		pwButtonTesting.EventEnter.Connect(ChangeCoverButtonEnter, pwButtonTesting);
		pwButtonTesting.EventLeave.Connect(ChangeCoverButtonLeave, pwButtonTesting);
		pwButtonTesting.EventClicked.Connect(StartTesting);

		Gui.GetCurrent().AddChild(pMainMenu, Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER);		//добавляем элемент на интерфейс

		EngineWindowViewport ewv = WindowManager.MainWindow;
		ewv.EventResized.Connect(ChangeMenuPosition);			//привязываем обработчик изменения размера окна приложения

	}

	void Update()
	{
		if (Input.IsKeyDown(Input.KEY.ESC) && !test.getActiveTest())
		{
			isMenuOpened = !isMenuOpened;
			pMainMenu.Hidden = !pMainMenu.Hidden;
			Input.MouseGrab = !Input.MouseGrab;			//меняем параметр захвата мышки на противоположный
			Gui gui;
			gui = Gui.GetCurrent();
			gui.MouseShow = !gui.MouseShow;				//включаем или отключаем отображение курсора
		}
		else if (isMenuOpened)			//пока меню открыто, при нажатии в пустое будем отключать захват мышки (для поворота камеры) и будем включать курсор, 
		{								//так как иначе при нажатии в пустое место они исчезают
			Input.MouseGrab = false;
			Gui gui;
			gui = Gui.GetCurrent();
			gui.MouseShow = true;
		}
	}


	private void Disk()
	{
		EngineWindowViewport ewv = WindowManager.MainWindow;
		ewv.EventResized.Disconnect(ChangeMenuPosition);			//отвязываем обработчик изменения размера окна приложения

		HideMenu();
		Console.Run("world_load Study_Project_SDO");       			 //сценарий Обслуживание

	}

	private void Work()
	{
		EngineWindowViewport ewv = WindowManager.MainWindow;
		ewv.EventResized.Disconnect(ChangeMenuPosition);			//отвязываем обработчик изменения размера окна приложения

		HideMenu();
		Console.Run("world_load Работа");
	}

	private void StartTesting()
	{
		isMenuOpened = false;
		pMainMenu.Hidden = true;
		test.StartTesting();
	}

	private void Exit()
	{
		Engine.Quit();
	}


	private void HideMenu()
	{
		isMenuOpened = false;
		pMainMenu.Hidden = true;
	}

	public void ShowMenu()
	{
		isMenuOpened = true;
		pMainMenu.Hidden = false;
	}

	private void ChangeCoverButtonEnter(WidgetSprite widget)			//сдвигаем виджет вниз, ак как у нас внизу спрайта кнопки находится наведенное состояние
	{
		widget.SetLayerTexCoord(0, new vec4(0.0f, 0.5f, 1.0f, 1f));
	}

	private void ChangeCoverButtonLeave(WidgetSprite widget)			//сдвигаем виджет вверх, так как у нас вверху спрайта кнопки находится ненаведенное состояние
	{
		widget.SetLayerTexCoord(0, new vec4(0.0f, 0f, 1.0f, 0.5f));
	}
	
	
	private void ChangeMenuPosition()
	{
		var mainWindowSize = WindowManager.MainWindow.Size;
		var px = pMainMenu.Width;
		var py = pMainMenu.Height;
		pMainMenu.SetPosition(mainWindowSize.x / 2 - px / 2, mainWindowSize.y / 2 - py / 2);	//устанавливаем новые размеры для главного меню
	}
}