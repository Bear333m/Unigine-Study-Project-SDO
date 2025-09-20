using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "bd4103b15a5c90ff98cb6c5a5703c7cdb860a302")]
public class TaskBase : Component
{
	[ShowInEditor]
	[Parameter(Title = "Название сценария")]
	private string taskName = null;

	[ShowInEditor]
	[Parameter(Title = "Список взаимодействующих объектов")]
	private List<Node> interactableObjects = new();				//список нод в том порядке, в котором с ними надо провзаимодействовать в сценарие

	[ShowInEditor]
	[Parameter(Title = "Список нод-позиций")]
	private List<Node> placeholderObjects = new();				//список нод-маркеров для перемещения на их место перемещаемых нод

	[ShowInEditor]
	[Parameter(Title = "Список текста для задачи")]
	private List<string> taskText = new();						//список строк для виджета с текстовым полем для отображения задачи на текущем этапе сценария

	private bool isTaskGoing = false;
	private int indexPlaceholder, indexTask = -1;
	private Dictionary<Node, dmat4> initialDrugObjects = new();


	[ShowInEditor]
	private CameraCast cameraCast = null;

	Gui gui;
	WidgetLabel labelTask, labelTooltip;


	[ShowInEditor]
	[Parameter(Title = "Вспомогательный скрипт")]
	private HelpComponent helpComponent = null;

	ObjectMeshStatic meshObject;


	void Init()
	{
		CreateUITask();         //создаем окошко слева сверху для отображения названия сценария и задания из текущего этапа сценария
		CreateUITooltip();      //создаем тектовую подсказку в центре экрана, которая появляется при наведении на объекты для взаимодействия
		for (int i = 0; i < interactableObjects.Count; i++)
		{
			if (!interactableObjects[i].GetComponent<ObjectProperties>().IsStaticObject())      //интерактивные, но не неперемещаемые объекты
			{
				if (!initialDrugObjects.ContainsKey(interactableObjects[i]))                    //если в справочнике нету объекта,
					initialDrugObjects.Add(                                                     // то добавляем его как ключ, а также мировую матрицу преобразований этого объекта
						interactableObjects[i],
						interactableObjects[i].WorldTransform
					);
			}
		}
	}

	void Update()
	{
		Object obj = cameraCast.GetObject();
		if (obj == null || obj.GetComponent<ObjectProperties>() == null) labelTooltip.Text = "";    //return;	//очищаем подсказку если никакой объект не выбран или к нему не прикреплен компонент ObjectProperties
		else labelTooltip.Text = obj.GetComponent<ObjectProperties>().GetText();                    //получаем текст из публичного тектового поля в компоненте ObjectProperties

		if (!isTaskGoing)
		{
			if (Input.IsKeyPressed(Input.KEY.P))        //Проверка нажатия клавиши "P" для запуска сценария
			{
				TaskStart();
			}
		}

		if (indexTask >= 0 && indexTask < interactableObjects.Count) {
			ObjectMeshStatic meshObject = (ObjectMeshStatic)interactableObjects[indexTask];
			SetOutline(meshObject, 1, new vec3(0.75f, 0.75f, 1.0f));                    //включаем подсветку желтым цветом для текущей ноды, с которой надо прозаимодействовать
		}
	}



	private void CreateUITask()
	{
		gui = Gui.GetCurrent();

		WidgetSprite background = new();
		background.Lifetime = Widget.LIFETIME.WORLD;
		background.Texture = "../data/gui/background_task.png";
		background.SetPosition(0, 0);
		background.Width = 300;
		background.Height = 200;

		labelTask = new();
		labelTask.Lifetime = Widget.LIFETIME.WORLD;
		labelTask.Text = taskName + "\n\nНажмите \"P\" для начала";
		labelTask.FontSize = 25;
		labelTask.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		labelTask.SetPosition(10, 10);
		labelTask.Width = 280;
		labelTask.Height = 180;
		labelTask.FontWrap = 1;

		gui.AddChild(background, Gui.ALIGN_OVERLAP | Gui.ALIGN_TOP | Gui.ALIGN_LEFT);
		gui.AddChild(labelTask, Gui.ALIGN_OVERLAP | Gui.ALIGN_TOP | Gui.ALIGN_LEFT);
	}

	private void CreateUITooltip()
	{
		labelTooltip = new();
		labelTooltip.Lifetime = Widget.LIFETIME.WORLD;
		labelTooltip.Width = 300;
		labelTooltip.TextAlign = 1;
		labelTooltip.Text = "";
		labelTooltip.FontSize = 20;
		labelTooltip.FontColor = new vec4(1.00, 0.46, 0.00, 1.0);

		gui.AddChild(labelTooltip, Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER);

		var mainWindowSize = WindowManager.MainWindow.Size;
		labelTooltip.PositionY = mainWindowSize.y / 3;
	}


	private void TaskStart()
	{
		foreach (var obj in initialDrugObjects)
		{
			obj.Key.WorldTransform = obj.Value;
		}

		isTaskGoing = true;
		indexPlaceholder = -1;
		indexTask = -1;
		TaskStage();

		if (helpComponent != null)          //Надо эту проверку ставить тут, ведь если поставить после foreach, может не успеть проинициализирвоаться некоторые вещи
		{
			helpComponent.RestartTask();
		}
	}

	public void TaskStage()
	{
		indexTask++;		//продвигаемся вперед

		if (helpComponent != null && indexTask > 0)
		{
			helpComponent.doneTask(interactableObjects[indexTask - 1]);		//отмечаем для сценария Работа, что предыдущий этап выполнен
		}

		if (indexTask >= taskText.Count)		//если индекс жтапа сценария превышает колиество строк в списке с тектом задания, то заканчиваем сценарий
		{
			TaskComplete();
			return;
		}
		labelTask.Text = taskText[indexTask];

		if (!interactableObjects[indexTask].GetComponent<ObjectProperties>().IsStaticObject())		//если объект не является статичным, то меняем ноду-маркер
		{
			indexPlaceholder++;
			ChangePlaceholderState();
		}

	}

	private void TaskComplete()
	{
		isTaskGoing = false;
		labelTask.Text = "Задача завершена. Нажмите \"P\" для продолжения";
		labelTooltip.Text = "";
	}



	public void ChangePlaceholderState()
	{
		placeholderObjects[indexPlaceholder].Enabled = !placeholderObjects[indexPlaceholder].Enabled;
	}

	public bool CheckTaskNode(Object selectedObject)
	{
		if (indexTask >= 0 && indexTask < interactableObjects.Count)
		{
			return interactableObjects[indexTask] == selectedObject;
		}
		return false;
	}

	public bool CheckDistance(Object selectedObject, float thresholdDistance)
	{
		double distance = (selectedObject.WorldBoundBox.Center - placeholderObjects[indexPlaceholder].WorldBoundBox.Center).Length;
		return distance < thresholdDistance;
	}

	public void SetPositionPlaceholder(Object obj)
	{
		obj.WorldTransform = placeholderObjects[indexPlaceholder].WorldTransform;
	}

	public void SetTooltipText()
	{
		labelTooltip.Text = interactableObjects[indexTask].GetComponent<ObjectProperties>().GetText();
	}

	/*	не используется
	public void ClearTooltipText()
	{
		labelTooltip.Text = "";
	}
	*/
	


	private void SetOutline(Object gameObject, int isOutline, vec3 color)
	{
		for (var i = 0; i < gameObject.NumSurfaces; i++)            //включаем обводку для всех поверхностей объекта
		{
			gameObject.SetMaterialState("auxiliary", isOutline, i);
			gameObject.SetMaterialParameterFloat3("auxiliary_color", color, i);
		}
	}
}