using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "4c937590ab1b9f368940a10a0ece2516adb786e3")]
public class CameraCast : Component
{
	[ShowInEditor]
	[Parameter(Title = "Камера игрока")]
	private PlayerDummy shootingCamera = null;

	[ShowInEditor]
	[ParameterMask(MaskType = ParameterMaskAttribute.TYPE.INTERSECTION)]
	[Parameter(Title = "Mаска пересечения")]
	private int mask = ~0;

	private dvec3 p0, p1;
	private Object selectedObject;
	private Object pastObject;

	public bool isDragged = false;

	private Gui gui;
	private WidgetLabel textLable;

	void Init()
	{
		CreateUIName();		//создаем текстовое поле для имени объекта в интерфейсе
	}

	void Update()
	{
		//Log.Message("Полученный объект: {0}\n", selectedObject);		//для вывода текста в консоль. Консоль открывается на ` (ё) на клавиатуре
		//Console.OnscreenMessage("Текст");

		selectedObject = GetObject();
		if (selectedObject && selectedObject.RootNode.Name == "dynamic_content")        //проверяем наведены ли мы на какой-то объект (до второго условия после провала первого не доходит)
		{
			SetOutline(selectedObject, 1, new vec3(1.0f, 1.0f, 1.0f));		//включаем обводку объекта белым цветом
			textLable.Text = selectedObject.Name;							//выводим в интерфейс имя выбранного объекта
		}
		if (pastObject != selectedObject)		//если мы больше не наведены на последний объект
		{
			if (pastObject != null)
			{
				SetOutline(pastObject, 0, new vec3(1.0f, 1.0f, 1.0f));		//выключаем обводку объекта белым цветом
				textLable.Text = "";
			}
			pastObject = selectedObject;
		}
	}



	public Object GetObject()
	{
		p0 = shootingCamera.WorldPosition;
		p1 = shootingCamera.WorldPosition + shootingCamera.GetWorldDirection() * 5.0f;		//строим точку в пространстве прибавляя к текущему положения камеры вектор направления камеры домноженный на множитель
		WorldIntersection intersection = new WorldIntersection();
		Object obj = World.GetIntersection(p0, p1, mask, intersection);						//получаем объект, который пересек вектор, выпущенный из нашей камеры
		return obj;
	}

	private void SetOutline(Object gameObject, int isOutline, vec3 color)
	{
		for (var i = 0; i < gameObject.NumSurfaces; i++)			//включаем обводку для всех поверхностей объекта
		{
			gameObject.SetMaterialState("auxiliary", isOutline, i);
			gameObject.SetMaterialParameterFloat3("auxiliary_color", color, i);
		}
	}


	public dmat4 GetIWorldTransform()
	{
		return shootingCamera.IWorldTransform;
	}

	public dmat4 GetOldWorldTransform()
	{
		return shootingCamera.OldWorldTransform;
	}
	
	private void CreateUIName()
	{
		gui = Gui.GetCurrent();

		textLable = new WidgetLabel(gui, "");
		textLable.Lifetime = Widget.LIFETIME.WORLD;
		textLable.FontSize = 30;
		textLable.FontColor = new vec4(1.0, 1.0, 1.0, 1.0);
		textLable.PositionX = 10;
		textLable.PositionY = -10;
		gui.AddChild(textLable, Gui.ALIGN_OVERLAP | Gui.ALIGN_BOTTOM | Gui.ALIGN_LEFT);
	}
}