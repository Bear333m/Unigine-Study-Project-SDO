using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "a9b1ce9184fecaff05353d1cd94551ccd196b3e9")]
public class Crosshair : Component
{
	[ShowInEditor]
	[Parameter(Title = "Изображение прицела")]
	private AssetLink crosshairImage = null;

	[ShowInEditor]
	[Parameter(Title = "Размер прицела")]
	private int crosshairSize = 16;

	private WidgetSprite crosshairWidget;
	private ivec2 prev_size = new ivec2(0, 0);
	ivec2 new_size;
	private Gui gui;


	void Init()
	{
		gui = Gui.GetCurrent();

		crosshairWidget = new WidgetSprite(gui, crosshairImage.AbsolutePath);
		crosshairWidget.Width = crosshairSize;
		crosshairWidget.Height = crosshairSize;
		crosshairWidget.Lifetime = Widget.LIFETIME.WORLD;
		gui.AddChild(crosshairWidget, Gui.ALIGN_CENTER);		// убрал флаг Gui.ALIGN_OVERLAP, что бы прицел не перекрывал главное меню и тест
	}
	
	void Update()
	{
		new_size = gui.Size;
		if (prev_size != new_size)			//если размер окна изменился, то удаляем видже и добавляем его снова заново выравнивая по центру
		{
			gui.RemoveChild(crosshairWidget);
			gui.AddChild(crosshairWidget, Gui.ALIGN_CENTER);
		}
		prev_size = new_size;
		
	}
}