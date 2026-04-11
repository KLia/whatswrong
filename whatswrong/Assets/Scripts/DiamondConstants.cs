public static class DiamondConstants
{
	/// <summary>
	/// These are the different string inputs values that can be given to the diamond.
	/// In this Simple Diamond Example, the Diamond only has one input "genre" which is a
	/// categorical. The categorical only has these 3 values. You can change which one to use
	/// to get different outputs. 
	/// </summary>
	public const string INPUT_ACTION = "action";
	public const string INPUT_HORROR = "horror";
	public const string INPUT_COMEDY = "comedy";
	
	/// <summary>
	/// This is the key for communicating to the diamond which input key to assign which values.
	/// In this Simple Diamond Example, we only have one input.
	/// </summary>
	public const string INPUT_KEY_GENRE = "genre";
	
	/// <summary>
	/// These are the Output values used to grab the text the Diamond has been generating
	/// based on the input values. 
	/// </summary>
	public const string OUTPUT_CHARACTER_A_REPLY = "character_a_reply";
	public const string OUTPUT_CHARACTER_B_REPLY = "character_b_reply";
}
