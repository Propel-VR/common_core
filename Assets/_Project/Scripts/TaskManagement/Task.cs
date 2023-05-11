using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using UnityEngine.Events;

namespace LM.TaskManagement
{
    #region Supplementary Types

    public enum TaskCheckType
    {
        Inspection = 0,
        Procedural
    }

    public enum TaskRectificationType
    {
        None = 0,
        Clean,
        Service,
        Record
    }

    public enum TaskRequiredConditionType
    {
        General = 0,
        Gloves,
        FaceShield,
        HeadProtection,
        EyeProtection,
        EarProtection,
        RespiratoryProtection,
        ProtectiveApron,
        FullProtectiveClothing,
        HighVisClothing,
        PowerOff,
        PowerOn,
        Disconnect,
        DisconnectElectrical,
        Ventilation,
        CheckGuard,
        Lock,
        FODControl,
        Ladder,
        Tool,
        Ruler,
        CleaningRag,
        CrewSupport,
        MultipleCrewSupport,
        ReferToManual
    }

    public enum TaskWarningType
    {
        Default = 0,
        Fire,
        Electrical,
        ExtremeCold,
        Explosion,
        Toxic
    }

    public enum TaskCautionType
    {
        Default = 0
    }

    [Serializable]
    public class TaskRequiredCondition
    {
        public TaskRequiredConditionType type;

        [TextArea(3, 8)]
        public string description;
    }

    [Serializable]
    public class TaskWarning
    {
        public TaskWarningType type;

        [TextArea(3, 8)]
        public string description;
    }

    [Serializable]
    public class TaskCaution
    {
        public TaskCautionType type; 

        [TextArea(3, 8)]
        public string description;
    }

    #endregion


    [CreateAssetMenu(fileName = "Task", menuName = "Tasks/Task", order = 1)]
    public class Task : ScriptableObject
    {
        #region Serialized Fields

        /// --- General
        [SerializeField]
        [TabGroup("General")]
        string _name;

        [SerializeField]
        [TextArea, TabGroup("General")]
        string _description;

        [SerializeField]
        [TabGroup("General")]
        TaskCategory _category;

        [SerializeField]
        [TabGroup("General")]
        TaskSubModule _subModule;

        [SerializeField]
        [TabGroup("General")]
        TaskModule _module;

        [SerializeField]
        [TabGroup("General")]
        int _orderWithinSubmodule;

        [SerializeField]
        [TabGroup("General")]
        TaskCheckType _checkType;

        [SerializeField]
        [TabGroup("General")]
        bool _canDefect;

        [SerializeField]
        [TabGroup("General"), ShowIf("_canDefect", true)]
        TaskRectificationType _rectificationType;

        /// --- Details
        [SerializeField]
        [TabGroup("Details")]
        string _SMIN;

        [SerializeField]
        [TabGroup("Details")]
        int _zone;

        [SerializeField]
        [TabGroup("Details")]
        float _manMinutes;

        [SerializeField]
        [TabGroup("Details")]
        string _trade;

        [SerializeField]
        [TabGroup("Details")]
        string _pwr;

        [SerializeField]
        [TabGroup("Other"), ListDrawerSettings(Expanded = true)]
        public List<TaskWarning> _warnings;

        [Space]
        [SerializeField]
        [TabGroup("Other"), ListDrawerSettings(Expanded = true)]
        public List<TaskCaution> _cautions;

        [Space]
        [SerializeField]
        [TabGroup("Other"), ListDrawerSettings(Expanded = true)]
        public List<TaskRequiredCondition> _requiredConditions;

        [Space]
        [SerializeField]
        [TabGroup("Other"), TextArea(3, 8), ListDrawerSettings(Expanded = true)]
        public List<string> _notes;

        #endregion

        #region Public Accessors

        /// --- General
        public string Name => _name;

        public string Description => _description;

        public TaskCategory Category => _category;

        public TaskSubModule SubModule => _subModule;

        public TaskModule Module => _module;

        public int OrderWithinSubmodule => _orderWithinSubmodule;

        public TaskCheckType CheckType => _checkType;

        public bool CanDefect => _canDefect;

        public TaskRectificationType RectificationType => _rectificationType;

        /// --- Details
        public string SMIN => _SMIN;

        public int Zone => _zone;

        public float ManMinutes => _manMinutes;

        public string Trade => _trade;

        public string Pwr => _pwr;

        public List<TaskWarning> Warnings => _warnings;

        public List<TaskCaution> Cautions => _cautions;

        public List<TaskRequiredCondition> RequiredConditions => _requiredConditions;

        public List<string> Notes => _notes;

        #endregion

    }

}
