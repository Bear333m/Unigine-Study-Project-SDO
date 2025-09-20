using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "bd4a9bc86cabcddd485d63eee3a9334d32bf852e")]
public class DrugObject : Component
{
	[ShowInEditor]
	private CameraCast cameraCast = null;

	private Dictionary<Object, (dmat4, dvec3)> initialDrugObjects = new();

	private Object selectedObject, draggedObject = null;
	private float thresholdDistance = 0.6f;         //расстояние, на котором предмет начинает возвращаться в исходное положение или же на ноду-маркер
	private dmat4 transform;

	[ShowInEditor]
	private TaskBase taskBase = null;


	void Init()
	{

	}

	void Update()
	{
		selectedObject = cameraCast.GetObject();

		if (selectedObject != null && taskBase.CheckTaskNode(selectedObject))		//если выбранный объект, является нынешним объектом для взаимодейтсвия на текущем этапе сценария
		{
			if (selectedObject.GetComponent<ObjectProperties>().IsStaticObject())	//для взаимодействия со статичным объектами нажимаем "E"
			{
				taskBase.SetTooltipText();				//выставляем в текстовое поле с подсказкой в интерфейсе текст подсказки из компонента ObjectProperties этого объекта
				if (Input.IsKeyDown(Input.KEY.E))
				{
					taskBase.TaskStage();
				}
			}
			else if (draggedObject != null)
			{
				/*	хз зачем это тут, ниже есть такой же код
				if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT))
				{
					cameraCast.isDragged = true;
					draggedObject = selectedObject;
					UpdateTransform(draggedObject);
				}
				*/
				if (Input.IsMouseButtonUp(Input.MOUSE_BUTTON.LEFT))
				{
					if (taskBase.CheckDistance(draggedObject, thresholdDistance))		//проверяем расстояние между перемещаемым объектом и зеленой нодой-маркером из задания
					{
						taskBase.SetPositionPlaceholder(draggedObject);					//устанавливаем удерживаемый объект на место ноду-маркера
						taskBase.ChangePlaceholderState();								//отключаем ноду-маркер
						taskBase.TaskStage();											//переходим на следующий этап сценария
					}
					cameraCast.isDragged = false;
					draggedObject = null;
				}
			}
		}


		if (cameraCast.isDragged)						//пока объект перемещается
		{
			if (Input.IsMouseButtonPressed(Input.MOUSE_BUTTON.LEFT))
			{
				HandleDraggedObject(draggedObject);
			}

			if (Input.IsMouseButtonUp(Input.MOUSE_BUTTON.LEFT))		//срабатывает, если до этого предмет не был отпущен вблизи зеленой полупрозрочной ноды из задания
			{
				SetInitialObject(draggedObject);		//пробуем поставить предмет на изначальное положение в сценарие
				cameraCast.isDragged = false;
				draggedObject = null;
			}
		}
		else if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT))		//если прожато ЛКМ, родительская нода это dynamic_content, объект не является статичным, то начинаем перемещение объекта
		{
			if (selectedObject != null && selectedObject.RootNode.Name == "dynamic_content" &&  selectedObject.GetComponent<ObjectProperties>() != null && !selectedObject.GetComponent<ObjectProperties>().IsStaticObject())        //до второго условия после провала первого не доходит
			{
				cameraCast.isDragged = true;
				draggedObject = selectedObject;
				AddInitialObject(draggedObject);
				UpdateTransform(draggedObject);
			}
		}
	}



	private void AddInitialObject(Object obj)
	{
		if (!initialDrugObjects.ContainsKey(obj))
		{
			initialDrugObjects.Add(obj, (obj.WorldTransform, obj.WorldBoundBox.Center));	//добавляем в спровачник значения положения и центра объекта по ключу в виде объекта
		}
	}

	public void SetInitialObject(Object obj)
	{
		if (initialDrugObjects.TryGetValue(obj, out var initialObject))
		{
			double distance = (obj.WorldBoundBox.Center - initialObject.Item2).Length;	//расстояние от удерживаемого предмета до его начальной позиции (по центру объекта)
			if (distance < thresholdDistance)											//если расстояние меньше заданного, то устанавливаем положение объекта в начальное
			{
				obj.WorldTransform = initialObject.Item1;
			}
		}
	}

	private void UpdateTransform(Object obj)						//выполняется при начале захвата объекта, то есть при опускании ЛКМ
	{
		dmat4 cameraTransform = cameraCast.GetIWorldTransform();	//локальные координаты камеры, полученные их мировых координат
		transform = cameraTransform * obj.WorldTransform;			//положение объекта относительно камеры (в системе координат камеры)
	}

	private void HandleDraggedObject(Object obj)					//выполняется пока объект перемещаеся, то есть пока ЛКМ зажат
	{
		obj.WorldTransform = cameraCast.GetOldWorldTransform() * transform;		//получаем мировую матрицу преобразований камеры с предыдущего кадра, умножаем положение объекта в координатах камеры, и получаем положение объекта в мировых координатах
	}
	
	
}