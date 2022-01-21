/*==============================================================================
Copyright (c) 2020 PTC Inc. All Rights Reserved.

Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;
using Vuforia;

/// <summary>
/// A custom handler that registers to the ObserverBehaviour callbacks
///
/// Changes made to this file could be overwritten when upgrading the Vuforia version.
/// When implementing custom event handler behavior, consider inheriting from this class instead.
/// </summary>
public class ObserverStatusEventHandler : DefaultObserverEventHandler
{
    TargetStatus mPreviousStatus;
    TargetStatus mCurrentStatus;

    bool TargetStatusIsTracked => mCurrentStatus.Status == Status.TRACKED ||
                                    mCurrentStatus.Status == Status.EXTENDED_TRACKED;

    bool TargetStatusIsNotTracked => mPreviousStatus.Status == Status.TRACKED &&
                                       (mCurrentStatus.Status == Status.NO_POSE ||
                                        mCurrentStatus.Status == Status.LIMITED);

    protected new virtual void Start()
    {
        mObserverBehaviour = GetComponent<ObserverBehaviour>();

        if (mObserverBehaviour)
        {
            mObserverBehaviour.OnTargetStatusChanged += TargetStatusChanged;
            mObserverBehaviour.OnBehaviourDestroyed += BehaviourDestroyed;
            
            TargetStatusChanged(mObserverBehaviour, mObserverBehaviour.TargetStatus);
        }
    }

    protected new virtual void OnDestroy()
    {
        if (mObserverBehaviour)
            BehaviourDestroyed(mObserverBehaviour);
    }

    void BehaviourDestroyed(ObserverBehaviour behaviour)
    {
        mObserverBehaviour.OnTargetStatusChanged -= TargetStatusChanged;
        mObserverBehaviour.OnBehaviourDestroyed -= BehaviourDestroyed;
        mObserverBehaviour = null;
    }

    void TargetStatusChanged(ObserverBehaviour behaviour, TargetStatus newStatus)
    {
        mPreviousStatus = mCurrentStatus;
        mCurrentStatus = newStatus;
        
        Debug.Log($"Observer { mObserverBehaviour.TargetName } " +
                  $"{ mObserverBehaviour.TargetStatus.Status } -- { mObserverBehaviour.TargetStatus.StatusInfo }");
        
        HandleTargetStatusChanged();
    }

    protected void HandleTargetStatusChanged()
    {
        if (TargetStatusIsTracked)
            OnTrackingFound();
        else if (TargetStatusIsNotTracked)
            OnTrackingLost();
        else
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
    }
}
