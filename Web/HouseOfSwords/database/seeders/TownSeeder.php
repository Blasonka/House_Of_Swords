<?php

namespace Database\Seeders;

use Illuminate\Database\Console\Seeds\WithoutModelEvents;
use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class TownSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        DB::table('towns')->updateOrInsert([ 'TownID' => 1 ],
        [
            'TownName' => 'Blasi városa',
            'XCords' => rand(-1000, 1000),
            'YCords' => rand(-1000, 1000),
            'Users_UID' => 1
        ]);
    }
}
