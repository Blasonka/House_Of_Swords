<?php

namespace Database\Seeders;

// use Illuminate\Database\Console\Seeds\WithoutModelEvents;

use Database\Seeders\LevelstatsSeeders\ChurchStatsSeeder;
use Database\Seeders\LevelstatsSeeders\InfirmaryStatsSeeder;
use Database\Seeders\LevelstatsSeeders\MarketStatsSeeder;
use Illuminate\Database\Seeder;

class DatabaseSeeder extends Seeder
{
    /**
     * Seed the application's database.
     *
     * @return void
     */
    public function run()
    {
        // \App\Models\User::factory(10)->create();

        // \App\Models\User::factory()->create([
        //     'name' => 'Test User',
        //     'email' => 'test@example.com',
        // ]);

        $this->call([
            UserSeeder::class,
            TownSeeder::class,
            BuildingSeeder::class,
            FriendListSeeder::class,
            BugReportSeeder::class,

            ChurchStatsSeeder::class,
            InfirmaryStatsSeeder::class,
            MarketStatsSeeder::class
        ]);
    }
}
