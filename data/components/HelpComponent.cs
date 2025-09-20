using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "182a9008a4e3c1e3869026e8e12aaf86acc7c1d9")]
public class HelpComponent : Component
{
	[ShowInEditor]
	private TrackPlayback trackPlayback = null;

	[ShowInEditor]
	[ParameterSlider(Title = "Изменяемые объекты", Group = "Ноды")]		//список нод, которые будут исчезать/появляться/двишаться во время сценария
	private List<Node> changingObjects;

	[ShowInEditor]
	[Parameter(Title = "Частицы")]
	private Node particleNode = null;

	private float animBaseTrackTime = 0.0f;

	private Dictionary<Node, (bool, dmat4)> initialChangingObjects = new();		//справочник для нод из списка выше, но еще с полями: включена ли нода на сцене, мировая матрица преобразований


	void Init()
	{
		foreach (Node changingObject in changingObjects)		// добавляем в справочник initialChangingObjects объекты из листа changingObject, также добавив поля: Включен ли объект, мировая матрица преобразвоаний
		{
			initialChangingObjects.Add(changingObject, (changingObject.Enabled, changingObject.WorldTransform));
		}
	}
	
	void Update()
	{
		animBaseTrackTime = trackPlayback.GetAnimBaseTrackTime();

		if (animBaseTrackTime >= 1.95f)
		{
			StopAnimBase();						//останавливаем анимацию плеча с пилой под конец таймера
		}

		if (animBaseTrackTime > 0.2f && animBaseTrackTime < 1.5f)
		{
			particleNode.Enabled = true;		//включаем ноду с частицами
		}

		if (animBaseTrackTime > 1.5f)
		{
			particleNode.Enabled = false;		//выключаем ноду с частицами
		}

		if (animBaseTrackTime > 1.0f)			//откючаем ноду с Заготовкой, и включаем разрезанные части заготовки
		{
			changingObjects[5].Enabled = false;
			changingObjects[6].Enabled = true;
			changingObjects[7].Enabled = true;
		}
	}



	public void RestartTask()
	{
		for (int i = 0; i < changingObjects.Count; i++)			//возвращаем все объекты в начальные позиции
		{
			changingObjects[i].Enabled = initialChangingObjects[changingObjects[i]].Item1;
			changingObjects[i].WorldTransform = initialChangingObjects[changingObjects[i]].Item2;
		}
	}

	public void StartAnimBase()
	{
		trackPlayback.StartAnim("animBase");
	}

	public void StartAnimDisk()
	{
		trackPlayback.StartAnim("animDisk");
	}

	public void StopAnimBase()
	{
		trackPlayback.StopAnim("animBase");
	}

	public void StopAnimDisk()
	{
		trackPlayback.StopAnim("animDisk");
	}



	public void doneTask(Node Object)			//вспомогаельные действия для сценария Работа
	{
		if (Object == changingObjects[0])		//отключаем Очки из предыдущего (только что выполненного) этапа
		{
			changingObjects[0].Enabled = false;
			return;
		}

		if (Object == changingObjects[1])		//двигаем Тиски
		{
			changingObjects[1].Position = new dvec3(changingObjects[1].Position.x + 0.095,
												changingObjects[1].Position.y,
												changingObjects[1].Position.z);
			return;
		}

		if (Object == changingObjects[2])		//двигаем кнопки Влючения и Выключения
		{
			changingObjects[2].Position = new dvec3(changingObjects[2].Position.x + 0.005,
												changingObjects[2].Position.y,
												changingObjects[2].Position.z);
			changingObjects[4].Position = new dvec3(changingObjects[4].Position.x - 0.005,
												changingObjects[4].Position.y,
												changingObjects[4].Position.z);
			StartAnimDisk();
			return;
		}

		if (Object == changingObjects[3])		//включаем анимацию Плеча с пилой
		{
			StartAnimBase();
			return;
		}

		if (Object == changingObjects[4])		//двигаем кнопки Влючения и Выключения
		{
			changingObjects[2].Position = new dvec3(changingObjects[2].Position.x - 0.005,
												changingObjects[2].Position.y,
												changingObjects[2].Position.z);
			changingObjects[4].Position = new dvec3(changingObjects[4].Position.x + 0.005,
												changingObjects[4].Position.y,
												changingObjects[4].Position.z);
			StopAnimDisk();         //останавливаем анимацию диска 
			return;
		}
	}

}