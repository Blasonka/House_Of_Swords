<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('levelstats_diplomacy', function (Blueprint $table) {
            $table->id('Lvl');

            $table->integer('MaxAllyCount')->unsigned();
            $table->integer('MaxAllyRange')->unsigned();
        });
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::dropIfExists('levelstats_diplomacy');
    }
};
