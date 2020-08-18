using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{

    [SerializeField, Tooltip("Shield Full Health Color (Default)")]
    private Color ShieldFullColor;

    [SerializeField, Tooltip("Shield Low Health Color")]
    private Color ShieldDamageColor;

    [SerializeField, Range(0.0f, 1.0f), Tooltip("Representation of how much health the shield has (High = low health)")]
    private float ShieldDamage;

    [SerializeField, Tooltip("Used as a backup for Instanced Materials (Only Edit if Shader is being changed)")]
    private Shader ShieldInstanceShader;

    private Light _shieldLight;
    private Material _shieldMat;
    private Renderer _renderer;

    public void TakeShieldDamage(float amount)
    {
        this.ShieldDamage = amount;
    }

    private void Awake()
    {
        _shieldLight = this.GetComponentInChildren<Light>();
        _renderer = this.GetComponent<Renderer>();
        _shieldMat = _renderer.material;
    }
    
    private void Update()
    {
        if (_shieldLight != null)
        {
            _shieldLight.color = Color.Lerp(ShieldFullColor, ShieldDamageColor, ShieldDamage);
        }

        if (_shieldMat != null)
        {
            _shieldMat.SetColor("_Color", ShieldFullColor);
            _shieldMat.SetColor("_LowColor", ShieldDamageColor);
            _shieldMat.SetFloat("_ShieldHealth", ShieldDamage);
        }
        else
        {
            _shieldMat = _renderer.material;
            if (_shieldMat == null)
            {
                _shieldMat = new Material(ShieldInstanceShader);
                _renderer.material = _shieldMat;
            }
        }
    }
}
