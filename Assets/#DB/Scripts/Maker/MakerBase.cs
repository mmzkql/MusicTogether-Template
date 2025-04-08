using UnityEngine;

namespace MusicTogether.DancingBall
{
    
    public class MakerBase// : MonoBehaviour
    {
        public InheritOption inheritOption;
        //public bool inheritFromLast;
        //public bool inheritFromSpecified;
        public int specifiedInheritIndex;
        public int targetIndex;
        public virtual void GetData(int selfIndex)
        {
            targetIndex = selfIndex;
            switch(inheritOption)
            {
                case InheritOption.Last:
                    targetIndex = selfIndex-1;
                    break;
                case InheritOption.Specified:
                    targetIndex = specifiedInheritIndex;
                    break;
                case InheritOption.noInherit:
                    return;
            }
        }
        private abstract void SetData();
    }
}