using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeleteAllData : MonoBehaviour {

    public GameObject confirmationMenu;

    public void DeleteAll() {
        confirmationMenu.SetActive(true);
    }

    public void Confirm() {
        SaveManager.DeleteAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Cancel() {
        confirmationMenu.SetActive(false);
    }
}
