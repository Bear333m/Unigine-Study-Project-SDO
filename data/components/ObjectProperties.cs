using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "52accaf0440dea72d8edf4ab2e9c0da46994586c")]
public class ObjectProperties : Component
{
	[ShowInEditor]
	[Parameter(Title = "Объект неперемещаемый")]
	private bool isStaticObject = false;

	[ShowInEditor]
	[Parameter(Title = "Текст")]
	[ParameterCondition("staticObject", 1)]
	private string text = "Текст";


	void Init()
	{
		// write here code to be called on component initialization

	}

	void Update()
	{
		// write here code to be called before updating each render frame

	}
	

	public string GetText()
	{
		return text;
	}

	public bool IsStaticObject()
	{
		return isStaticObject;
	}
}