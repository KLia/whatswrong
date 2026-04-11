public static class DiamondConstants
{
	/// <summary>
	/// These are the different string inputs values that can be given to the diamond.
	/// In this Simple Diamond Example, the Diamond only has one input "genre" which is a
	/// categorical. The categorical only has these 3 values. You can change which one to use
	/// to get different outputs. 
	/// </summary>
	public const string INPUT_HAPPY = "happy";
	public const string INPUT_SAD = "sad";
	public const string INPUT_WAR = "war";
	
	/// <summary>
	/// This is the key for communicating to the diamond which input key to assign which values.
	/// In this Simple Diamond Example, we only have one input.
	/// </summary>
	public const string INPUT_KEY_ORIGINAL = "original_story";
	public const string INPUT_KEY_TONE = "desired_tone";
	
	/// <summary>
	/// These are the Output values used to grab the text the Diamond has been generating
	/// based on the input values. 
	/// </summary>
	public const string OUTPUT_ENDING = "alternate_ending";
}
