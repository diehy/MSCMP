using HutongGames.PlayMaker;

namespace MSCMP.Extensions
{
    public static class PlayMakerExtensions
    {
        public static void RemoveActionsAfter( this FsmState fs, int index )
        {
            var array = new FsmStateAction[index + 1];
            for ( int i = 0; i <= index; i++ )
            {
                array[i] = fs.Actions[i];
            }

            fs.Actions = array;
        }
    }
}
