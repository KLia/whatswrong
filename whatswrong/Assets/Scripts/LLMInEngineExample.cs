using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using RawPowerLabs.DynamicAI;
using Random = System.Random;

public class LLMInEngineExample : MonoBehaviour
{
	// /// <summary>
	// /// This is the class needed to generate the text from the LLM used inside the Engine.
	// /// We keep it as a global field, so we can create it and use it at two different times.
	// /// You are welcome to use it however you like. 
	// /// </summary>
	// private CloudTextModule _cloudTextModule;
	//
	// /// <summary>
	// /// // Start is called once before the first execution of Update method after the MonoBehaviour is created.
	// /// This means that this code will run as the first thing after hitting the play button inside of Unity.
	// /// </summary>
 //    void Start()
 //    {
	//     // To use a LLM in engine we also need a context object, that contains the base context
	//     // for how the text should be generated.
	//     var context = new RawPowerLabs.DynamicAI.Context();
	//     
	//     // Dev
	//     /*var cloudTextModuleOptions = new CloudTextModuleOptions();
	//     cloudTextModuleOptions.AifactoryUrl = "https://aifactory-backend.development.rptexternal.com";
	//     cloudTextModuleOptions.DesignmoduleUrl = "https://design-module-backend.development.rptexternal.com";
	//     cloudTextModuleOptions.ZitadelUrl = "https://dev-2vel1z.us1.zitadel.cloud";
	//     cloudTextModuleOptions.ZitadelProjectId = "322746757345752454";
	//     cloudTextModuleOptions.ProjectId = "019d6829-b1a2-7b17-88c9-34031071febc";
	//     cloudTextModuleOptions.ClientId = "ngj_api";
	//     cloudTextModuleOptions.ClientSecret = "5gswWI0G0BPElCtPBkfzVnNc6PhZ7ihfZfz0zWmjZRn86kyiY4Oafz6qACmxrjnd";
	//     cloudTextModuleOptions.Model = "gpt-5.2";*/
	//     
	//     // Below you find the needed values in order to use the LLM in Engine feature inside of Unity.
	//     // Remember to change the ProjectId to be the correct project ID for your project on the platform.
	//     // Remember this feature only works by using the VPN.
	//     
	//     // Production
	//     var cloudTextModuleOptions = new CloudTextModuleOptions();
	//     cloudTextModuleOptions.AifactoryUrl = "https://aifactory-backend.platform.dynamic-ai.net";
	//     cloudTextModuleOptions.DesignmoduleUrl = "https://design-module-backend.platform.dynamic-ai.net";
	//     cloudTextModuleOptions.ZitadelUrl = "https://prod-w3rgvu.eu1.zitadel.cloud";
	//     cloudTextModuleOptions.ZitadelProjectId = "364907271840357777";
	//     cloudTextModuleOptions.ProjectId = "019d6829-b1a2-7b17-88c9-34031071febc";
	//     cloudTextModuleOptions.ClientId = "ngj_api";
	//     cloudTextModuleOptions.ClientSecret = "vk0mVVo6cyV2MRN41t2dSVpVFFcRYqwazXo8O1iZdiVDL1EOC4cX86HveL0fp8ty";
	//     cloudTextModuleOptions.Model = "gpt-5.2";
	//     
	//     // To create the text module, we create it from the context created above.
	//     // we need to use the CloudTextModuleOptions object created above.
	//     _cloudTextModule = context.CreateCloudTextModule(cloudTextModuleOptions);
	//     
	//     PrintReplies("The bride killed her husband for cheating on her", DiamondConstants.INPUT_SAD);
 //    }
 //
	// /// <summary>
	// /// In order to make sure the game is still running while the LLM is generating the reply
	// /// we call the logic using async method. This will allow us to still keep playing animation etc.
	// /// while waiting for the reply from the LLM.
	// /// </summary>
	// /// <param name="original_story"></param>
	// /// <param name="tone"></param>
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
	// 	if (_cloudTextModule == null)
	// 	{
	// 		UnityEngine.Debug.LogError("_cloudTextModule is null");
	// 		return null;
	// 	}
	//     
	// 	// In order to set the inputs that will be used by the LLM
	// 	// we need a TextModuleInput class. We can create this by using the
	// 	// following method from the TextModule class.
	// 	// We use the "using" keyword for the variable, to tell the compile
	// 	// to clean up the references once we are done using the variable.
	// 	// If not, we would need to clean up this ourselves.
	// 	using var textModuleInput = _cloudTextModule.CreateInput();
	// 	
	// 	// Here we tell the input module which key we want to set.
	// 	// If you have more than one input then invoking the LLM you can set
	// 	// them by calling the .Set method with each key and the value
	// 	// for the input. The SDK currently allows to set strings and integer
	// 	// values.
	// 	textModuleInput.Set(DiamondConstants.INPUT_KEY_ORIGINAL, original_story);
	// 	textModuleInput.Set(DiamondConstants.INPUT_KEY_TYPE, tone);
	//     
	// 	// To get more fine-grained control over how the LLM should invoke
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
	// 	// generated by the LLM. If this value is not set, the default behavior
	// 	// is still to randomize, such that the end result is different each time. 
	// 	var random = new Random();
	// 	invokeParameters.Seed = (uint) random.Next(0, int.MaxValue);
	//     
	// 	// Once we have set up both the TextModuleInvokeParameters and the inputs
	// 	// we can invoke the LLM. Depending on the size of the model, this is
	// 	// the operation that will take some time.
	// 	// Again we use the "using" keyword to let the compiler now, that we want it
	// 	// to clean up the object once we are done using it.
	// 	// Remember to use VPN in order for this feature to work.
	// 	using var textResult = _cloudTextModule.Invoke(invokeParameters, textModuleInput);
	// 	
	// 	var replies = new Dictionary<string, string>();
	// 	
	// 	// Once the TexModule class is done invoking the input parameters,
	// 	// we will have an object of the class TextResult. This object can be used
	// 	// to get the output values.
	// 	// In the same way as with inputs, we get the string values of each of the outputs by
	// 	// using the correct key value.
	// 	var ending = textResult.GetString(DiamondConstants.OUTPUT_ENDING);
	// 	
	// 	// In this example I'm using a Dictionary to keep both key and values
	// 	// together, but you can use the values however you want.
	// 	replies.Add(DiamondConstants.OUTPUT_ENDING, ending);
	//     
	// 	return replies;
	// }
}
