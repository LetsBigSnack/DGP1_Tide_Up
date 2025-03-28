using System.Collections.Generic;
using UnityEngine;

public class TestUtil : MonoBehaviour
{
    [SerializeField] private List<TrashData> listOfDummyTrash = new();

    public List<TrashData> ListOfDummyTrash
    {
        get { return listOfDummyTrash; }
        set { listOfDummyTrash = value; }
    }
}
