using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewContractIndicator
{
    private City.ContractState state;
    private float rotationSpeed = 0.75f;
    private float totalRotation = 0.0f;
    private float maxRotation = 10.0f;
    private GameObject myObject;
    private Coroutine activeRoutine;

    public NewContractIndicator(GameObject feedbackObject)
    {
        myObject = feedbackObject;
    }

    public void SetState(City.ContractState newState)
    {
        state = newState;
        if (activeRoutine != null)
        {
            UberManager.Instance.StopCoroutine(activeRoutine);
            activeRoutine = null;
        }
        switch (newState)
        {
            case City.ContractState.New:
                activeRoutine = UberManager.Instance.StartCoroutine(NewContracts());
                break;
            case City.ContractState.Seen:
                activeRoutine = UberManager.Instance.StartCoroutine(SeenContracts());
                break;
            case City.ContractState.Nothing:
                activeRoutine = UberManager.Instance.StartCoroutine(NoContracts());
                break;
        }
    }

    private IEnumerator NewContracts()
    {
        myObject.SetActive(true);
        while (state == City.ContractState.New)
        {
            totalRotation += rotationSpeed;
            if (totalRotation >= maxRotation || totalRotation <= -maxRotation)
            {
                rotationSpeed *= -1;
            }
            myObject.transform.Rotate(new Vector3(0.0f, 0.0f, rotationSpeed));
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }

    private IEnumerator SeenContracts()
    {
        myObject.transform.localEulerAngles = Vector3.zero;
        myObject.SetActive(true);
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator NoContracts()
    {
        myObject.SetActive(false);
        yield return new WaitForEndOfFrame();
    }
}
