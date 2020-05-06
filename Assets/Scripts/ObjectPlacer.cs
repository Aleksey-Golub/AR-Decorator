using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private Transform _objectPlace;                    // место установки
    [SerializeField] private Camera _camera;                            // камера для пускания лучей
    [SerializeField] private GameObject _container;                     // контейнер 

    private ARRaycastManager _arRaycastManager;                         // связь с ARRaycastManager
    private GameObject _installedOblect;                                // объект, выбранный для установки
    private List<ARRaycastHit> _hits = new List<ARRaycastHit>();        //список для рэйкастов

    private void Start()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
    }

    private void Update()
    {
        UpdatePlacementPose();

        if (Input.touchCount == 2)
        {
            SetObject();
        }
    }

    private void UpdatePlacementPose()      // позволяет водить предметом по полу
    {
        Vector2 screenCenter = _camera.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));      // получение точки - центра экрана, т.к. распознавание пола происходит точкой в центре экрана??

        var ray = _camera.ScreenPointToRay(screenCenter);       // создание луча для рэйкаста

        if (Physics.Raycast(ray, out RaycastHit raycastHit))    // рэйкаст луча и сбор встреченных объектов-КОЛЛАЙДЕРОВ в raycastHit
        {
            SetObjectPosition(raycastHit.point);
        }
        else if(_arRaycastManager.Raycast(screenCenter, _hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            SetObjectPosition(_hits[0].pose.position);
        }
    }

    private void SetObjectPosition(Vector3 position)
    {
        _objectPlace.position = position;

        Vector3 cameraForward = _camera.transform.forward;
        Vector3 cameraRotation = new Vector3(cameraForward.x, 0, cameraForward.z);
        _objectPlace.rotation = Quaternion.Euler(cameraRotation);
    }

    private void SetObject()
    {
        _installedOblect.GetComponent<Collider>().enabled = true;       // включение коллайдера после его выключения(чтобы луч в него не упирался)
        _installedOblect.transform.parent = _container.transform;       // смена родителя устанавливаемого предмета
        _installedOblect = null;
    }

    public void SetInstalledObject(ItemData itemData)
    {
        if (_installedOblect != null)
            Destroy(_installedOblect);

        _installedOblect = Instantiate(itemData.Prefab, _objectPlace);
        _installedOblect.GetComponent<Collider>().enabled = false;
    }
}
