using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using RawPowerLabs.DynamicAI;
using RawPowerLabs.DynamicAI.Utility;
using Random = System.Random;

public class DiamondExample : MonoBehaviour
{
	// /// <summary>
	// /// Write the name of the folder that contains the diamond.
	// /// This will allow us to find the diamond using our Utility function.
	// /// You are welcome to fetch the path in however way you want.
	// /// Just remember the Diamond files _needs_ to be in the Streaming Assets folder
	// /// Otherwise they will not be included in a build.
	// /// </summary>
	// [SerializeField]
	// private string _diamondName;
	//
	// /// <summary>
	// /// This is the class needed to generate the text from a Diamond.
	// /// We keep it as a global field, so we can create it and use it at two different times.
	// /// You are welcome to use it however you like. 
	// /// </summary>
	// private TextModule  _textModule;
	//
	//
	// /// <summary>
	// /// // Start is called once before the first execution of Update method after the MonoBehaviour is created.
	// /// This means that this code will run as the first thing after hitting the play button inside of Unity.
	// /// </summary>
 //    void Start()
 //    {
	//     // In order to use a Diamond, we first need the path to the model (.gguf) and the template (.json) files.
	//     // In order to make sure the Diamond gets included in a build opf the game, the assets _needs_ to be placed
	//     // in side the Streaming Assets folder. This Utility class makes it easy for you to fetch the path. 
	//     var diamondPath = DiamondUtility.GetPathFromDiamondName(_diamondName);
	//     
	//     // To use a Diamond we also need a context object, that contains the base context
	//     // for how the diamond should be used.
	//     var context = new RawPowerLabs.DynamicAI.Context();
	//     
	//     // The Context object makes use of some parameters, that can be changed in regard to how the text should be generated.
	//     // This would include if we should use the GPU or not when generating the text. For now using the default parameters
	//     // will do just fine.
	//     var parameters = TextModuleParameters.GetDefault();
	//     
	//     // To create the text module, we create it from the context created above.
	//     // we need to use the parameters, and the path to the diamond files.
	//     _textModule = context.CreateTextModule(parameters, diamondPath.modelPath, diamondPath.templatePath);
	//     
	//     // in case the model (.gguf) file is not found, the value of the TextModule class will be null.
	//     // If that is the case in this context, that would be a mistake. So we have a null check, to make sure
	//     // the diamond is set up correct. Calling this, will also load up the diamond into memory.
	//     if (_textModule == null)
	//     {
	// 	    UnityEngine.Debug.LogError("Something went wrong with the Diamond");
	//     }
	//     
	//     PrintReplies("The bride decided to kiss her husband and forgive him", DiamondConstants.INPUT_WAR);
 //    }
 //
	// /// <summary>
	// /// In order to make sure the game is still running while the diamond is getting the reply
	// /// we call the Diamond using async method. This will allow us to still keep playing animation etc.
	// /// while waiting for the reply from the Diamond.
	// /// </summary>
	// /// <param name="input"></param>
	// private async void PrintReplies(string original_story, string tone)
	// {
	// 	var replies = new Dictionary<string, string>();
 //
	// 	try
	// 	{
	// 		replies = await InvokeReplyAsync(original_story, tone);
	// 	}
	// 	catch (Exception e)
	// 	{
	// 		UnityEngine.Debug.LogException(e);
	// 	}
 //
	// 	foreach (var reply in replies)
	// 	{
	// 		UnityEngine.Debug.Log($"Key: {reply.Key}, Value: {reply.Value}");
	// 	}
	// }
	//
	// private async Task<Dictionary<string, string>> InvokeReplyAsync(string original_story, string tone)
	// {
	// 	return await Task.Run(() => InvokeReply(original_story, tone));
	// }
	//
	// private Dictionary<string, string> InvokeReply(string original_story, string tone)
	// {
	// 	if (_textModule == null)
	// 	{
	// 		UnityEngine.Debug.LogError("_textModule is null");
	// 		return null;
	// 	}
	//     
	// 	// In order to set the inputs that will be used by the Diamond
	// 	// we need a TextModuleInput class. We can create this by using the
	// 	// following method from the TextModule class.
	// 	// We use the "using" keyword for the variable, to tell the compile
	// 	// to clean up the references once we are done using the variable.
	// 	// If not, we would need to clean up this ourselves.
	// 	using var textModuleInput = _textModule.CreateInput();
	// 	
	// 	// Here we tell the input module which key we want to set.
	// 	// If you have more than one input to your Diamond you can set
	// 	// them by calling the .Set method with each key and the value
	// 	// for the input. The SDK currently allows to set strings and integer
	// 	// values.
	// 	textModuleInput.Set(DiamondConstants.INPUT_KEY_ORIGINAL, original_story);
	// 	textModuleInput.Set(DiamondConstants.INPUT_KEY_TYPE, tone);
	//     
	// 	// To get more fine-grained control over how the Diamond should invoke
	// 	// the inputs, we also have the following TextModuleInvokeParameters class.
	// 	// For this we can also get the default parameters.
	// 	var invokeParameters = TextModuleInvokeParameters.GetDefault();
	// 	
	// 	// For now, it is important to remember to set the Predict Count, since
	// 	// this value is 0 by default. This value controls the size of window that
	// 	// the model is allowed to look forward when generating the different tokens
	// 	// used for creating the text. If the value is zero, the generated text will be empty.
	// 	// Try experiment with what value creates the best result for you.
	// 	invokeParameters.PredictCount = 1024;
	// 	
	// 	// Here we set the seed, which guarantee that we will have different answers
	// 	// generated by the Diamond. If this value is not set, the default behavior
	// 	// is still to randomize, such that the end result is different each time. 
	// 	var random = new Random();
	// 	invokeParameters.Seed = (uint) random.Next(0, int.MaxValue);
	//     
	// 	// Once we have set up both the TextModuleInvokeParameters and the inputs
	// 	// we can invoke the Diamond. Depending on the size of the diamond, this is
	// 	// the operation that will take some time.
	// 	// Again we use the "using" keyword to let the compiler now, that we want it
	// 	// to clean up the object once we are done using it.
	// 	using var textResult = _textModule.Invoke(invokeParameters, textModuleInput);
	// 	
	// 	var replies = new Dictionary<string, string>();
	// 	
	// 	// Once the TexModule class is done invoking the input parameters,
	// 	// we will have an object of the class TextResult. This object can be used
	// 	// to get the output values.
	// 	// In the same way as with inputs, we get the string values of each of the outputs by
	// 	// using the correct key value.
	// 	var alternate	= textResult.GetString(DiamondConstants.OUTPUT_ENDING);
	// 	
	// 	
	// 	// In this example I'm using a Dictionary to keep both key and values
	// 	// together, but you can use the values however you want.
	// 	replies.Add(DiamondConstants.OUTPUT_ENDING, alternate);
	//     
	// 	return replies;
	// }
}
