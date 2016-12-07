using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelGeneration : MonoBehaviour {

    public int _seed = 1235;
    [Header("Setup")]
    [Tooltip("The Ground")]
    public GameObject _ground;
    [Tooltip("Stuff on the ice")]
    public GameObject[] _scenery;
    [Tooltip("Stuff on the snow")]
    public GameObject[] _scenerySnow;
    [Tooltip("carrots: the first one is the small carrot, second is the big carrot")]
    public GameObject[] _carrot;
    [Tooltip("An empty object for holding all the stuff, will be a child of the level")]
    public GameObject _objectHolder;
    public InputField _inputField;

    [Header("Scale, Offest, Location stuff")]
    [Range(0, 1), Tooltip("Minimal scale of stuff")]
    public double _seneryScaleMin = 0.7;
    [Range(1, 5), Tooltip("Maximal scale of stuff")]
    public double _seneryScaleMax = 1.5;
    [Range(-10, 0), Tooltip("Minimal offset of stuff in Y direction")]
    public double _objectYOffsetMin = 0;
    [Range(0, 10), Tooltip("Maximal offset of stuff in Y direction")]
    public double _objectYOffsetMax = 3;
    //public double _snowEnd1 = -15;
    [Range(-30, 0), Tooltip("The location of boundary of ice and the snow of left hand side")]
    public double _snowIceboundary1 = -10;
    [Range(0, 30), Tooltip("The location of boundary of ice and the snow of right hand side")]
    public double _snowIceboundary2 = 10;

    [Header("Y Direction")]
    [Range(0, 20), Tooltip("Minimal dist the Y 'pointer' will move")]
    public double _movementYMin = 6.2;
    [Range(0, 20), Tooltip("Maximal dist the Y 'pointer' will move")]
    public double _movementYMax = 7.8;
    [Range(0, 100), Tooltip("Starting point of the Y 'pointer'")]
    public double _movementYStart = 6;
    [Range(0, 100), Tooltip("Location where the Scenery will instantiate in Y direction")]
    public double _movementYSceneryStart = 20;
    [Range(0, 100), Tooltip("Location where the Scenery will stop instantiate in Y direction")]
    public double _movementYEnd = 98;

    [Header("X Direction")]
    [Range(0, 20), Tooltip("Minimal dist the X 'pointer' will move on snow")]
    public double _movementXSnowMin = 2.8;
    [Range(0, 20), Tooltip("Maximal dist the X 'pointer' will move on snow")]
    public double _movementXSnowMax = 5;
    [Range(0, 20), Tooltip("Minimal dist the X 'pointer' will move on ice")]
    public double _movementXIceMin = 4;
    [Range(0, 20), Tooltip("Maximal dist the X 'pointer' will move on ice")]
    public double _movementXIceMax = 6;
    [Range(-30, 0), Tooltip("The location where stuff will instantiate on the snow at the left hand side in X direction")]
    public double _snowStart1 = -25;
    [Range(-10, 10), Tooltip("The location of stuff instantiation on the ice in X direction")]
    public double _iceStart = -9;
    [Range(0, 30), Tooltip("The location of stuff instantiation on the snow at the right hand side in X direction")]
    public double _snowStart2 = 15;
    [Range(-30, 30), Tooltip("Location where the Scenery will stop instantiate in X direction")]
    public double _movementXEnd = 30;

    [Header("Prob.")]
    [Range(0,1), Tooltip("Prob. to instantiate scenery")]
    public double _probScenery = 0.67;
    [Range(0, 1), Tooltip("Prob. to instantiate big carrot")]
    public double _probBigCarrot = 0.2;

    private Transform _parent;
    private Transform _groundTransform;
    private double _yCnt = 0;
    private double _xCnt = 0;
    private bool _done = false;
    

	// Use this for initialization
	void Awake () {
        _parent = _objectHolder.transform;
        _groundTransform = _ground.transform;
        _objectHolder.transform.position = _ground.transform.position;
        _objectHolder.transform.parent = _ground.transform.parent;
    }
	
	// Update is called once per frame
	void Update () {
        RandomGenerator.Seed(_seed);
        LevelGenerator();
    }

    public void SetSeed()
    {
        string num = _inputField.text;
        int seed = 0;
        for (int i = 0; i<num.Length; i++)
        {
            char character = num[i];
            seed *= 10;
            seed = seed + (int)char.GetNumericValue(character);
        }
        _seed = seed;
    }

    public void Reset()
    {
        foreach(Transform child in _objectHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        _done = false;
    }

    public void LevelGenerator()
    {
        if(!_done && _objectHolder == null)
        {
            _objectHolder = new GameObject("ObjectHolder");
        }
        _yCnt = _groundTransform.position.y + _movementYStart;
        while(!_done && _yCnt < _groundTransform.position.y + _movementYEnd)
        {
            _xCnt = _groundTransform.position.x + _snowStart1;
            while(!_done && _xCnt < _groundTransform.position.x + _movementXEnd)
            {
                double offset = RandomGenerator.Random_d(_objectYOffsetMin, _objectYOffsetMax);
                Vector3 position = new Vector3((float)_xCnt, (float)(_yCnt + RandomGenerator.Random_d(_objectYOffsetMin, _objectYOffsetMax)), 0);
                float scale = (float)RandomGenerator.Random_d(_seneryScaleMin, _seneryScaleMax);
                Quaternion rotation = Quaternion.identity;
                rotation.eulerAngles = new Vector3(0, 0, (float)RandomGenerator.Random_d(0.0, 359.0));

                if (_xCnt <= _groundTransform.position.x + _snowIceboundary1)
                {
                    int index = RandomGenerator.Random_i(0, _scenerySnow.Length - 1);
                    GameObject New = Instantiate(_scenerySnow[index], position, rotation, _parent) as GameObject;
                    New.transform.localScale *= scale;

                    _xCnt += RandomGenerator.Random_d(_movementXSnowMin, _movementXSnowMax);
                }
                else if(_xCnt <= _groundTransform.position.x + _snowIceboundary2)
                {
                    if(_xCnt < _groundTransform.position.x + _iceStart)
                    {
                        _xCnt = _groundTransform.position.x + _iceStart;
                    }
                    if(_yCnt <= _movementYSceneryStart)
                    {
                        double carrotProb = RandomGenerator.Random_d(0.0, 1.0);
                        int index = (carrotProb <= _probBigCarrot) ? 1 : 0;// index 0 is small carrot, 1 is big
                        GameObject New = Instantiate(_carrot[index], position, rotation, _parent) as GameObject;
                    }
                    else
                    {
                        double prob = RandomGenerator.Random_d(0.0, 1.0);
                        bool scenery = (prob <= _probScenery) ? true : false;
                        if(scenery)
                        {
                            int index = RandomGenerator.Random_i(0, _scenery.Length - 1);
                            GameObject New = Instantiate(_scenery[index], position, rotation, _parent) as GameObject;
                            New.transform.localScale *= scale;
                        }
                        else
                        {
                            double carrotProb = RandomGenerator.Random_d(0.0, 1.0);
                            int index = (carrotProb <= _probBigCarrot) ? 1 : 0;// index 0 is small carrot, 1 is big
                            GameObject New = Instantiate(_carrot[index], position, rotation, _parent) as GameObject;
                        }
                    }
                    
                    _xCnt += RandomGenerator.Random_d(_movementXIceMin, _movementXIceMax);
                }
                else
                {
                    if (_xCnt < _groundTransform.position.x + _snowStart2)
                    {
                        _xCnt = _groundTransform.position.x + _snowStart2;
                    }
                    int index = RandomGenerator.Random_i(0, _scenerySnow.Length - 1);
                    GameObject New = Instantiate(_scenerySnow[index], position, rotation, _parent) as GameObject;
                    New.transform.localScale *= scale;

                    _xCnt += RandomGenerator.Random_d(_movementXSnowMin, _movementXSnowMax);
                }
            }

            _yCnt += RandomGenerator.Random_d(_movementYMin, _movementYMax);

            if(_yCnt >= _movementYEnd)
            {
                _done = true;
            }
        }
    }
}
