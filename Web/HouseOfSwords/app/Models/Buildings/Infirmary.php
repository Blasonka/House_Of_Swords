<?php

namespace App\Models\Buildings;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Infirmary extends Model
{
    use HasFactory;

    // table properties
    protected $table = 'levelstats_infirmary';
    protected $primaryKey = 'Lvl';
    public $timestamps = false;

    protected $fillable = [
        'HealingTime',
        'Effectivity',
        'MaxInjuredUnits',
        'MaxHealedUnits'
    ];
}
