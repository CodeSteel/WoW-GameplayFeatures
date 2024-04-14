using System;

namespace MMO.ActionSystem
{
    [Serializable]
    public class ActionObject
    {
        public string ActionSoId;
        public float LastUseTime;

        private ActionSo _actionSo;
        
        public ActionObject()
        {
        }
        
        public ActionObject(ActionSo actionSo)
        {
            ActionSoId = actionSo.Id;
        }

        public ActionSo GetActionSo()
        {
            if (_actionSo != null) return _actionSo;
            ItemDatabaseHandlerData.GetActionById(ActionSoId, ref _actionSo);
            return _actionSo;
        }
    }
}