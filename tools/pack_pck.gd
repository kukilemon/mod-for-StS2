extends SceneTree

const RAW_FILES := [
	"EireneMod/localization/zhs/cards.json",
	"EireneMod/localization/zhs/characters.json",
	"EireneMod/localization/zhs/powers.json",
	"EireneMod/localization/zhs/relics.json",
	"scenes/creature_visuals/eirene.tscn",
	"scenes/ui/character_icons/eirene_icon.tscn",
	"scenes/combat/energy_counters/eirene_energy_counter.tscn",
	"scenes/merchant/characters/eirene_merchant.tscn",
	"scenes/rest_site/characters/eirene_rest_site.tscn",
	"scenes/vfx/card_trail_eirene.tscn",
	"scenes/screens/char_select/char_select_bg_eirene.tscn",
	"materials/transitions/eirene_transition_mat.tres",
	"images/atlases/card_atlas.sprites/ironclad/eirene_strike.tres",
	"images/atlases/card_atlas.sprites/ironclad/eirene_defend.tres",
	"images/atlases/card_atlas.sprites/ironclad/standard_lantern.tres",
	"images/atlases/card_atlas.sprites/ironclad/open_fire.tres",
	"images/atlases/card_atlas.sprites/ironclad/rapid_fire.tres",
	"images/atlases/card_atlas.sprites/ironclad/cruising_missile.tres",
	"images/atlases/card_atlas.sprites/ironclad/hold_breath.tres",
	"images/atlases/card_atlas.sprites/ironclad/rolling_shot.tres",
	"images/atlases/card_atlas.sprites/ironclad/suppressive_fire.tres",
	"images/atlases/card_atlas.sprites/ironclad/aim_for_the_vitals.tres",
	"images/atlases/card_atlas.sprites/ironclad/cover.tres",
	"images/atlases/card_atlas.sprites/ironclad/triple_shot.tres",
	"images/atlases/card_atlas.sprites/ironclad/bullet.tres",
	"images/atlases/card_atlas.sprites/ironclad/load_ammunition.tres",
	"images/atlases/card_atlas.sprites/ironclad/blinding_round.tres",
	"images/atlases/card_atlas.sprites/ironclad/concussion_round.tres",
	"images/atlases/card_atlas.sprites/ironclad/reinforced_cover.tres",
	"images/atlases/card_atlas.sprites/ironclad/homemade_ammunition.tres",
	"images/atlases/card_atlas.sprites/ironclad/quick_reload.tres",
	"images/atlases/card_atlas.sprites/ironclad/tracer_round.tres",
	"images/atlases/card_atlas.sprites/ironclad/explosive_round.tres",
	"images/atlases/card_atlas.sprites/ironclad/weapon_modification.tres",
	"images/atlases/card_atlas.sprites/ironclad/solemn_mourning.tres",
	"images/atlases/card_atlas.sprites/ironclad/double_shot_kit.tres",
	"images/atlases/card_atlas.sprites/ironclad/dual_wield_form.tres",
	"images/atlases/card_atlas.sprites/ironclad/death_death_death.tres",
	"images/atlases/card_atlas.sprites/ironclad/adjust_stance.tres",
	"images/atlases/card_atlas.sprites/ironclad/lunge_step.tres",
	"images/atlases/card_atlas.sprites/ironclad/rising_thrust.tres",
	"images/atlases/card_atlas.sprites/ironclad/pursuing_thrust.tres",
	"images/atlases/card_atlas.sprites/ironclad/armor_piercing_thrust.tres",
	"images/atlases/card_atlas.sprites/ironclad/aerial_pursuit.tres",
	"images/atlases/card_atlas.sprites/ironclad/fencing_etiquette.tres",
	"images/atlases/card_atlas.sprites/ironclad/advancing_slash.tres",
	"images/atlases/card_atlas.sprites/ironclad/sweeping_slash.tres",
	"images/atlases/card_atlas.sprites/ironclad/aerial_intercept.tres",
	"images/atlases/card_atlas.sprites/ironclad/perfect_parry.tres",
	"images/atlases/card_atlas.sprites/ironclad/launch.tres",
	"images/atlases/card_atlas.sprites/ironclad/chain_thrust.tres",
	"images/atlases/card_atlas.sprites/ironclad/defensive_counter.tres",
	"images/atlases/card_atlas.sprites/ironclad/moonlight_sword.tres",
	"images/atlases/card_atlas.sprites/ironclad/gravity_reversal.tres",
	"images/atlases/card_atlas.sprites/ironclad/sun_sword.tres",
	"images/atlases/card_atlas.sprites/ironclad/sword_wind_suppression.tres",
	"images/atlases/card_atlas.sprites/ironclad/duel_stance.tres",
	"images/atlases/card_atlas.sprites/ironclad/judgment.tres",
	"images/atlases/card_atlas.sprites/ironclad/sword_gun_concerto.tres",
	"images/atlases/card_atlas.sprites/ironclad/meteor_sword.tres",
	"images/atlases/card_atlas.sprites/ironclad/gale_final_thrust.tres",
	"images/atlases/card_atlas.sprites/ironclad/light_the_way.tres",
	"images/atlases/card_atlas.sprites/ironclad/prepare_under_lantern.tres",
	"images/atlases/card_atlas.sprites/ironclad/borrow_light.tres",
	"images/atlases/card_atlas.sprites/ironclad/light_match.tres",
	"images/atlases/card_atlas.sprites/ironclad/lantern_strike.tres",
	"images/atlases/card_atlas.sprites/ironclad/throw_spark.tres",
	"images/atlases/card_atlas.sprites/ironclad/disarm.tres",
	"images/atlases/card_atlas.sprites/ironclad/night_watch.tres",
	"images/atlases/card_atlas.sprites/ironclad/lantern_bearer.tres",
	"images/atlases/card_atlas.sprites/ironclad/recycle_embers.tres",
	"images/atlases/card_atlas.sprites/ironclad/guide.tres",
	"images/atlases/card_atlas.sprites/ironclad/recall_in_light.tres",
	"images/atlases/card_atlas.sprites/ironclad/lighting_ritual.tres",
	"images/atlases/card_atlas.sprites/ironclad/ember_shield.tres",
	"images/atlases/card_atlas.sprites/ironclad/guard_the_flame.tres",
	"images/atlases/card_atlas.sprites/ironclad/add_lamp_oil.tres",
	"images/atlases/card_atlas.sprites/ironclad/evolution_ritual.tres",
	"images/atlases/card_atlas.sprites/ironclad/eirene_metamorphosis.tres",
	"images/atlases/card_atlas.sprites/ironclad/holy_light_baptism.tres",
	"images/atlases/card_atlas.sprites/ironclad/brilliant_lantern.tres",
	"images/atlases/card_atlas.sprites/ironclad/throw_flame.tres",
	"images/atlases/card_atlas.sprites/ironclad/return_home.tres",
	"images/atlases/card_atlas.sprites/ironclad/expanded_lantern.tres",
	"images/atlases/card_atlas.sprites/ironclad/flame_impact.tres",
	"images/atlases/card_atlas.sprites/ironclad/final_form.tres",
	"images/atlases/card_atlas.sprites/ironclad/eternal_lantern.tres",
	"images/atlases/card_atlas.sprites/ironclad/sea_born_transformation.tres",
	"images/atlases/relic_atlas.sprites/church_rapier.tres",
	"images/atlases/relic_outline_atlas.sprites/church_rapier.tres",
	"images/atlases/power_atlas.sprites/lantern_power.tres",
	"images/atlases/power_atlas.sprites/precision_power.tres",
	"images/atlases/power_atlas.sprites/imbalance_power.tres",
	"images/atlases/power_atlas.sprites/floating_power.tres",
	"images/atlases/power_atlas.sprites/combo_power.tres",
	"images/atlases/power_atlas.sprites/lose_precision_power.tres",
	"images/atlases/power_atlas.sprites/reinforced_cover_power.tres",
	"images/atlases/power_atlas.sprites/quick_reload_power.tres",
	"images/atlases/power_atlas.sprites/explosive_round_power.tres",
	"images/atlases/power_atlas.sprites/double_shot_kit_power.tres",
	"images/atlases/power_atlas.sprites/dual_wield_form_power.tres",
	"images/atlases/power_atlas.sprites/adjust_stance_power.tres",
	"images/atlases/power_atlas.sprites/fencing_etiquette_power.tres",
	"images/atlases/power_atlas.sprites/perfect_parry_power.tres",
	"images/atlases/power_atlas.sprites/defensive_counter_power.tres",
	"images/atlases/power_atlas.sprites/moonlight_sword_power.tres",
	"images/atlases/power_atlas.sprites/gravity_reversal_power.tres",
	"images/atlases/power_atlas.sprites/sword_gun_concerto_power.tres",
	"images/atlases/power_atlas.sprites/meteor_sword_power.tres",
	"images/atlases/power_atlas.sprites/lantern_capacity_power.tres",
	"images/atlases/power_atlas.sprites/guide_draw_power.tres",
	"images/atlases/power_atlas.sprites/guide_block_power.tres",
	"images/atlases/power_atlas.sprites/lamp_oil_power.tres",
	"images/atlases/power_atlas.sprites/flame_impact_power.tres",
	"images/atlases/power_atlas.sprites/eternal_lantern_power.tres",
	"images/atlases/power_atlas.sprites/lantern_lock_power.tres",
	"images/atlases/power_atlas.sprites/floating_lock_power.tres",
	"images/atlases/power_atlas.sprites/sea_born_power.tres",
	"images/atlases/power_atlas.sprites/night_watch_cost_power.tres",
	"images/atlases/power_atlas.sprites/final_form_power.tres",
	"images/atlases/power_atlas.sprites/final_form_progress_power.tres",
]

const IMPORTED_TEXTURES := [
	"images/ui/top_panel/character_icon_eirene.png",
	"images/ui/top_panel/character_icon_eirene_outline.png",
	"images/packed/character_select/char_select_eirene.png",
	"images/packed/character_select/char_select_eirene_locked.png",
	"images/packed/map/icons/map_marker_eirene.png",
	"images/packed/card_portraits/ironclad/eirene_strike.png",
	"images/packed/card_portraits/ironclad/eirene_defend.png",
	"images/packed/card_portraits/ironclad/standard_lantern.png",
	"images/packed/card_portraits/ironclad/open_fire.png",
	"images/packed/card_portraits/ironclad/rapid_fire.png",
	"images/packed/card_portraits/ironclad/cruising_missile.png",
	"images/packed/card_portraits/ironclad/hold_breath.png",
	"images/packed/card_portraits/ironclad/rolling_shot.png",
	"images/packed/card_portraits/ironclad/suppressive_fire.png",
	"images/packed/card_portraits/ironclad/aim_for_the_vitals.png",
	"images/packed/card_portraits/ironclad/cover.png",
	"images/packed/card_portraits/ironclad/triple_shot.png",
	"images/packed/card_portraits/ironclad/bullet.png",
	"images/packed/card_portraits/ironclad/load_ammunition.png",
	"images/packed/card_portraits/ironclad/blinding_round.png",
	"images/packed/card_portraits/ironclad/concussion_round.png",
	"images/packed/card_portraits/ironclad/reinforced_cover.png",
	"images/packed/card_portraits/ironclad/homemade_ammunition.png",
	"images/packed/card_portraits/ironclad/quick_reload.png",
	"images/packed/card_portraits/ironclad/tracer_round.png",
	"images/packed/card_portraits/ironclad/explosive_round.png",
	"images/packed/card_portraits/ironclad/weapon_modification.png",
	"images/packed/card_portraits/ironclad/solemn_mourning.png",
	"images/packed/card_portraits/ironclad/double_shot_kit.png",
	"images/packed/card_portraits/ironclad/dual_wield_form.png",
	"images/packed/card_portraits/ironclad/death_death_death.png",
	"images/packed/card_portraits/ironclad/adjust_stance.png",
	"images/packed/card_portraits/ironclad/lunge_step.png",
	"images/packed/card_portraits/ironclad/rising_thrust.png",
	"images/packed/card_portraits/ironclad/pursuing_thrust.png",
	"images/packed/card_portraits/ironclad/armor_piercing_thrust.png",
	"images/packed/card_portraits/ironclad/aerial_pursuit.png",
	"images/packed/card_portraits/ironclad/fencing_etiquette.png",
	"images/packed/card_portraits/ironclad/advancing_slash.png",
	"images/packed/card_portraits/ironclad/sweeping_slash.png",
	"images/packed/card_portraits/ironclad/aerial_intercept.png",
	"images/packed/card_portraits/ironclad/perfect_parry.png",
	"images/packed/card_portraits/ironclad/launch.png",
	"images/packed/card_portraits/ironclad/chain_thrust.png",
	"images/packed/card_portraits/ironclad/defensive_counter.png",
	"images/packed/card_portraits/ironclad/moonlight_sword.png",
	"images/packed/card_portraits/ironclad/gravity_reversal.png",
	"images/packed/card_portraits/ironclad/sun_sword.png",
	"images/packed/card_portraits/ironclad/sword_wind_suppression.png",
	"images/packed/card_portraits/ironclad/duel_stance.png",
	"images/packed/card_portraits/ironclad/judgment.png",
	"images/packed/card_portraits/ironclad/sword_gun_concerto.png",
	"images/packed/card_portraits/ironclad/meteor_sword.png",
	"images/packed/card_portraits/ironclad/gale_final_thrust.png",
	"images/packed/card_portraits/ironclad/light_the_way.png",
	"images/packed/card_portraits/ironclad/prepare_under_lantern.png",
	"images/packed/card_portraits/ironclad/borrow_light.png",
	"images/packed/card_portraits/ironclad/light_match.png",
	"images/packed/card_portraits/ironclad/lantern_strike.png",
	"images/packed/card_portraits/ironclad/throw_spark.png",
	"images/packed/card_portraits/ironclad/disarm.png",
	"images/packed/card_portraits/ironclad/night_watch.png",
	"images/packed/card_portraits/ironclad/lantern_bearer.png",
	"images/packed/card_portraits/ironclad/recycle_embers.png",
	"images/packed/card_portraits/ironclad/guide.png",
	"images/packed/card_portraits/ironclad/recall_in_light.png",
	"images/packed/card_portraits/ironclad/lighting_ritual.png",
	"images/packed/card_portraits/ironclad/ember_shield.png",
	"images/packed/card_portraits/ironclad/guard_the_flame.png",
	"images/packed/card_portraits/ironclad/add_lamp_oil.png",
	"images/packed/card_portraits/ironclad/evolution_ritual.png",
	"images/packed/card_portraits/ironclad/eirene_metamorphosis.png",
	"images/packed/card_portraits/ironclad/holy_light_baptism.png",
	"images/packed/card_portraits/ironclad/brilliant_lantern.png",
	"images/packed/card_portraits/ironclad/throw_flame.png",
	"images/packed/card_portraits/ironclad/return_home.png",
	"images/packed/card_portraits/ironclad/expanded_lantern.png",
	"images/packed/card_portraits/ironclad/flame_impact.png",
	"images/packed/card_portraits/ironclad/final_form.png",
	"images/packed/card_portraits/ironclad/eternal_lantern.png",
	"images/packed/card_portraits/ironclad/sea_born_transformation.png",
	"images/relics/church_rapier.png",
	"images/powers/lantern_power.png",
	"images/powers/precision_power.png",
	"images/powers/imbalance_power.png",
	"images/powers/floating_power.png",
	"images/powers/combo_power.png",
	"images/powers/lose_precision_power.png",
	"images/powers/reinforced_cover_power.png",
	"images/powers/quick_reload_power.png",
	"images/powers/explosive_round_power.png",
	"images/powers/double_shot_kit_power.png",
	"images/powers/dual_wield_form_power.png",
	"images/powers/adjust_stance_power.png",
	"images/powers/fencing_etiquette_power.png",
	"images/powers/perfect_parry_power.png",
	"images/powers/defensive_counter_power.png",
	"images/powers/moonlight_sword_power.png",
	"images/powers/gravity_reversal_power.png",
	"images/powers/sword_gun_concerto_power.png",
	"images/powers/meteor_sword_power.png",
	"images/powers/lantern_capacity_power.png",
	"images/powers/guide_draw_power.png",
	"images/powers/guide_block_power.png",
	"images/powers/lamp_oil_power.png",
	"images/powers/flame_impact_power.png",
	"images/powers/eternal_lantern_power.png",
	"images/powers/lantern_lock_power.png",
	"images/powers/floating_lock_power.png",
	"images/powers/sea_born_power.png",
	"images/powers/night_watch_cost_power.png",
	"images/powers/final_form_power.png",
	"images/powers/final_form_progress_power.png",
]


func _initialize() -> void:
	var output_path := ProjectSettings.globalize_path("res://build/EireneMod.pck")
	var packer := PCKPacker.new()
	var error := packer.pck_start(output_path)
	if error != OK:
		push_error("Unable to create PCK: %s" % error_string(error))
		quit(1)
		return

	for relative_path in RAW_FILES:
		var source_path := ProjectSettings.globalize_path("res://" + relative_path)
		if not FileAccess.file_exists(source_path):
			push_error("Missing PCK input: " + source_path)
			quit(1)
			return

		error = packer.add_file("res://" + relative_path, source_path)
		if error != OK:
			push_error("Unable to add %s: %s" % [relative_path, error_string(error)])
			quit(1)
			return

	for relative_path in IMPORTED_TEXTURES:
		error = _add_imported_texture(packer, relative_path)
		if error != OK:
			quit(1)
			return

	error = packer.flush()
	if error != OK:
		push_error("Unable to finish PCK: %s" % error_string(error))
		quit(1)
		return

	print("Created raw-resource PCK: " + output_path)
	quit()


func _add_imported_texture(packer: PCKPacker, relative_path: String) -> Error:
	var import_relative_path := relative_path + ".import"
	var import_source_path := ProjectSettings.globalize_path("res://" + import_relative_path)
	if not FileAccess.file_exists(import_source_path):
		push_error("Missing texture import metadata: " + import_source_path)
		return ERR_FILE_NOT_FOUND

	var import_config := ConfigFile.new()
	var error := import_config.load(import_source_path)
	if error != OK:
		push_error("Unable to read %s: %s" % [import_relative_path, error_string(error)])
		return error

	var imported_res_path: String = import_config.get_value("remap", "path", "")
	if imported_res_path.is_empty():
		push_error("No remap path in " + import_relative_path)
		return ERR_INVALID_DATA

	var imported_source_path := ProjectSettings.globalize_path(imported_res_path)
	if not FileAccess.file_exists(imported_source_path):
		push_error("Missing imported texture: " + imported_source_path)
		return ERR_FILE_NOT_FOUND

	error = packer.add_file("res://" + import_relative_path, import_source_path)
	if error != OK:
		push_error("Unable to add %s: %s" % [import_relative_path, error_string(error)])
		return error

	error = packer.add_file(imported_res_path, imported_source_path)
	if error != OK:
		push_error("Unable to add %s: %s" % [imported_res_path, error_string(error)])
	return error
