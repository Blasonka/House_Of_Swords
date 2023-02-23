<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Building extends Model
{
    use HasFactory;

    // table properties
    protected $table = 'buildings';
    protected $primaryKey = 'BuildingID';
    public $timestamps = false;

    protected $fillable = [
        'Towns_TownID',
        'BuildingType',
        'BuildingLvl',
        'Params'
    ];

    // KAPCSOLATOK
    public function town(){
        return $this->belongsTo(Town::class, 'Towns_TownID', 'TownID');
    }
}
