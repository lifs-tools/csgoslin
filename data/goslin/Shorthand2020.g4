/*
 * MIT License
 * 
 * Copyright (c) the authors (listed in global LICENSE file)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the 'Software'), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:;
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHether IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
*/

/* This is a BNF / ANTLR4 grammar for lipid subspecies identifiers following
 * Liebisch et al. Volume 61, Issue 12, December 2020, Pages 1539-1555.
 */

grammar Shorthand2020;


/* first rule is always start rule */
lipid : lipid_eof EOF;
lipid_eof : lipid_pure | lipid_pure adduct_info;
lipid_pure : gl | pl | sl | sterol | med; /* glycero lipids, phospho lipids, sphingo lipids, sterol lipids, lipid mediators


/* adduct information */
adduct_info : adduct_sep | adduct_separator adduct_sep;
adduct_sep : '[M' adduct ']' charge_sign | '[M' adduct ']' charge charge_sign;
adduct : adduct_set;
adduct_set : adduct_element | adduct_element adduct_set;
adduct_element : element | element number | number element | plus_minus element | plus_minus element number | plus_minus number element;


/* mediators */
med : med_species | med_subspecies;
med_species : med_hg_double headgroup_separator fatty_acyl_chain | med_hg_triple headgroup_separator fatty_acyl_chain;
med_subspecies : med_hg_single headgroup_separator fatty_acyl_chain |
                 med_hg_double headgroup_separator fatty_acyl_chain2 |
                 med_hg_triple headgroup_separator fatty_acyl_chain3;
med_hg_single : 'FA' | 'FOH' | 'FAL' | 'CAR' | 'CoA' | 'NAE' | 'NAT' | 'WD' | 'HC' | 'FAHFA';
med_hg_double : 'WE' | 'NA';
med_hg_triple : 'WD';




/* fatty acyl chain */
lcb : fatty_acyl_chain;
fatty_acyl_chain : fatty_acyl | fatty_acyl stereo;
fatty_acyl : fa_pure | ether fa_pure | fatty_acyl_linkage | fatty_alkyl_linkage;
fatty_alkyl_linkage : fatty_linkage_number fatty_acyl_linkage_sign ROB fatty_acyl_chain RCB | fatty_acyl_linkage_sign ROB fatty_acyl_chain RCB;
fatty_acyl_linkage : fatty_linkage_number fatty_acyl_linkage_sign ROB med RCB | fatty_acyl_linkage_sign ROB med RCB;
hydrocarbon_chain : hydrocarbon_number ROB fatty_acyl_chain RCB | ROB fatty_acyl_chain RCB;
fatty_acyl_linkage_sign : 'O' | 'N';
fatty_linkage_number : number;
hydrocarbon_number : number;
fa_pure : fa_pure_structure | fa_pure_structure stereo | fa_pure_structure sn;
sn : ROB 'sn-' sn_pos RCB;
sn_pos : number;
fa_pure_structure : carbon carbon_db_separator db | carbon carbon_db_separator db db_funcgroup_separator func_group;
ether : ether_num ether_type | ether_type;
ether_num : 'm' | 'd' | 't' | 'e';
ether_type: ether_types plasmalogen_separator | ether_types plasmalogen_separator;
ether_types: 'O' | 'P';
carbon : number;
db : db_count | db_count db_positions;
db_count : number;
db_positions : ROB db_position RCB;
db_position : db_single_position | db_position db_position_separator db_position;
db_single_position : db_position_number | db_position_number cistrans;
db_position_number : number;
cistrans : 'E' | 'Z';

func_group : func_group_entity;
func_group_entity : func_group_entity funcgroup_separator func_group_entity | func_group_data | func_group_data_repetition;
func_group_data_repetition : func_group_data_repetition func_repetition_separator func_group_data_repetition | func_group_data;
func_group_data : func_group_name | func_group_cycle | func_group_pos func_group_ext_name | func_group_pos func_group_ext_name stereo | func_group_ext_count_name func_group_count | func_group_ext_name func_group_count stereo | molecular_func_group | fatty_acyl_linkage | fatty_alkyl_linkage | hydrocarbon_chain;
func_group_pos : func_group_pos_number | func_group_pos_number ring_stereo;
ring_stereo : 'a' | 'b';
func_group_pos_number : number;
func_group_count : number;
stereo : SOB stereo_number stereo_type SCB | SOB stereo_type SCB;
stereo_number : number;
stereo_type : 'R' | 'S';
molecular_func_group : molecular_func_group_name | molecular_func_group_name func_group_count;
func_group_ext_name : round_open_bracket func_group_name round_close_bracket | func_group_name;
func_group_ext_count_name : round_open_bracket func_group_name round_close_bracket | molecular_func_group_name;
func_group_name : 'Et' | 'Me' | 'Ac' | 'NO2' | 'My' | 'Ep' | 'OO' | 'dMe' | 'OMe' | 'oxy' | 'NH2' | 'OOH' | 'SH' | 'OH' | 'oxo' | 'CN' | 'Ph' | 'Su' | 'COOH' | 'G' | 'T' | 'COG' | 'COT' | carbohydrate | 'H' | 'Cys' | 'Phe' | 'SGlu' | 'SCys' | 'BOO' | 'MMAs' | 'SMe' | 'NH' | molecular_func_group_name | 'SCG';
molecular_func_group_name : elements;
elements : 'O' | 'N' | 'P' | 'S' | 'As' | 'Br' | 'Cl' | 'F' | 'I';
func_group_cycle : SOB cycle_base SCB | SOB cycle_base funcgroup_separator cycle_func_group_data SCB;
cycle_base : cycle_def | cycle_def carbon_db_separator cycle_db;
cycle_def : cycle_start cycle_separator cycle_end cycle_token cycle_number | cycle_start cycle_separator cycle_end cycle_bridge cycle_token cycle_number | cycle_bridge cycle_token cycle_number | cycle_token cycle_number;
cycle_bridge : cylce_elements;
cylce_elements : cylce_elements cylce_elements | cylce_element;
cylce_element : elements;
cycle_token : 'cy';
cycle_number : number;
cycle_start : number;
cycle_end : number;
cycle_func_group_data : func_group_entity;
cycle_db : cycle_db_cnt | cycle_db_cnt ROB cycle_db_positions RCB;
cycle_db_cnt : number;
cycle_db_positions : cycle_db_position;
cycle_db_position : cycle_db_position db_position_separator cycle_db_position | cycle_db_position_number | cycle_db_position_number cycle_db_position_cis_trans;
cycle_db_position_number : number;
cycle_db_position_cis_trans : cistrans;

fatty_acyl_chain2 : fa2_sorted | fa2_unsorted;
fa2_unsorted : fatty_acyl_chain unsorted_fa_separator fatty_acyl_chain;
fa2_sorted : fatty_acyl_chain sorted_fa_separator fatty_acyl_chain;

fatty_acyl_chain3 : f3_sorted | fa3_unsorted;
f3_sorted : fa2_sorted sorted_fa_separator fatty_acyl_chain;
fa3_unsorted : fa2_unsorted unsorted_fa_separator fatty_acyl_chain;

fatty_acyl_chain4 : f4_sorted | fa4_unsorted;
f4_sorted : fa2_sorted sorted_fa_separator fa2_sorted;
fa4_unsorted : fa2_unsorted unsorted_fa_separator fa2_unsorted;


/* glycero lipids */
gl : gl_species | gl_subpsecies | gl_molecular_species;
gl_species : gl_hg_dt headgroup_separator fatty_acyl_chain;
gl_molecular_species : gl_hg_double headgroup_separator fatty_acyl_chain2 | gl_hg_triple headgroup_separator fatty_acyl_chain2 | gl_hg_single headgroup_separator fatty_acyl_chain;
gl_subpsecies : gl_hg headgroup_separator fatty_acyl_chain3;
gl_hg_dt : gl_hg_double | gl_hg_true_double | gl_hg_triple;
gl_hg : gl_hg_single | gl_hg_double | gl_hg_true_double | gl_hg_triple;
gl_hg_single : 'MG' | 'MGMG' | 'DGMG' | 'SQMG';
gl_hg_double : 'DG';
gl_hg_true_double : 'MGDG' | 'DGDG' | 'SQDG';
gl_hg_triple : 'TG';



pl : pl_species | pl_subspecies | pl_molecular_species;
pl_species : pl_hg headgroup_separator fatty_acyl_chain;
pl_subspecies : pl_single | pl_double | pl_quadro;
pl_molecular_species : pl_hg_quadro headgroup_separator fatty_acyl_chain2 | pl_hg_quadro headgroup_separator fatty_acyl_chain3;
pl_single : pl_hg_single headgroup_separator fatty_acyl_chain;
pl_full : pl_hg_single headgroup_separator fatty_acyl_chain2;
pl_double : pl_full | pl_hg_double_all headgroup_separator fatty_acyl_chain2;
pl_quadro : pl_hg_quadro headgroup_separator fatty_acyl_chain4;
pl_hg : pl_hg_double_all | pl_hg_quadro;
pl_hg_single : 'LPA' | 'LPC' | 'LPE' | 'LPG' | 'LPI' | 'LPS' | hg_lpim | 'CPA' | 'LCDPDAG' | 'LDMPE' | 'LMMPE' | 'LPIMIP' | 'LPIN' | 'PE-isoLG';
pl_hg_double_all : pl_hg_double_fa | pl_hg_double | hg_pip;
pl_hg_double_fa : pl_hg_double_fa_hg ROB pl_hg_fa RCB | pl_hg_double_fa_hg ROB pl_hg_alk RCB;
pl_hg_double_fa_hg : 'PS-N' | 'PE-N';
pl_hg_double : 'CDP-DAG' | 'DMPE' | 'MMPE' | 'PA' | 'PC' | 'PE' | 'PEt' | 'PG' | 'PI' | 'PS' | 'LBPA' | 'PGP' | 'PPA' | 'Glc-GP' | '6-Ac-Glc-GP' | hg_pim | 'PnC' | 'PnE' | 'PT' | 'PE-NMe2' | 'PE-NMe' | 'PIMIP' | 'CDPDAG' | 'PS-CAP' | 'PS-MDA' | 'PE-CAP' | 'PE-Glc' | 'PE-GlcA' | 'PE-GlcK' | 'PE-CM' | 'PE-CE' | 'PE-FA' | 'PE-CA' | 'PE-MDA' | 'PE-HNE' | pl_hg_species;
pl_hg_species : hg_PE_PS ROB hg_PE_PS_type RCB;
hg_PE_PS : 'PE-N' | 'PS-N';
hg_PE_PS_type : 'Alk' | 'FA'; 
pl_hg_quadro : 'BMP' | 'CL' | 'LCL';
hg_pip : hg_pip_pure_m | hg_pip_pure_d | hg_pip_pure_t | hg_pip_pure_m ROB hg_pip_m RCB | hg_pip_pure_d ROB hg_pip_d RCB | hg_pip_pure_t ROB hg_pip_t RCB;
hg_pip_pure_m : 'PIP';
hg_pip_pure_d : 'PIP2';
hg_pip_pure_t : 'PIP3';
hg_pip_m : '3' APOSTROPH | '4' APOSTROPH | '5' APOSTROPH;
hg_pip_d : '3' APOSTROPH COMMA '4' APOSTROPH | '4' APOSTROPH COMMA '5' APOSTROPH | '3' APOSTROPH COMMA '5' APOSTROPH;
hg_pip_t : '3' APOSTROPH COMMA '4' APOSTROPH COMMA '5' APOSTROPH;
hg_pim : 'PIM' hg_pim_number;
hg_pim_number : number;
hg_lpim : 'LPIM' hg_lpim_number;
hg_lpim_number : number;
pl_hg_fa : med;
pl_hg_alk : fatty_acyl_chain;


carbohydrate : 'Hex' | 'Gal' | 'Glc' | 'Man' | 'Neu' | 'HexNAc' | 'GalNAc' | 'GlcNAc' | 'NeuAc' | 'NeuGc' | 'Kdn' | 'GlcA' | 'Xyl' | 'Fuc' | 'NeuAc2' | 'SHex' | 'S' ROB '3' APOSTROPH RCB 'Hex' | 'NAc' | 'Nac' | 'SGal' | 'S' ROB '3' APOSTROPH RCB 'Gal' | 'HexA' | 'OGlcNAc' | 'OGlc';


sl : sl_species | sl_subspecies;
sl_species : sl_hg_double headgroup_separator lcb | acer_species headgroup_separator lcb;
sl_subspecies : sl_hg_single headgroup_separator lcb | sl_hg_single sl_hydroxyl headgroup_separator lcb | sl_double;
sl_double : sl_hg_double headgroup_separator lcb sorted_fa_separator fatty_acyl_chain | sl_hg_double sl_hydroxyl headgroup_separator lcb sorted_fa_separator fatty_acyl_chain;
sl_hydroxyl : ROB sl_hydroxyl_number RCB;
sl_hydroxyl_number : number;
sl_hg_single : 'SPB' | 'SPBP' | 'LIPC' | 'LSM';
sl_hg_double : acer_hg | sl_hg_double_name | carbohydrate_structural sl_hg_double  | carbohydrate_isomeric sl_hg_double;
carbohydrate_structural : carbohydrate;
carbohydrate_isomeric : carbohydrate carbohydrate_separator;
sl_hg_double_name : 'SM' | 'Cer' | 'CerP' | acer_hg | 'HexCer' | 'GlcCer' | 'GalCer' | 'Hex2Cer' | 'LacCer' | 'SHexCer' | 'IPC' | 'PI-Cer' | 'EPC' | 'PE-Cer' | 'GIPC' | 'MIPC' | 'M(IP)2C' | 'Hex3Cer' | 'S' ROB '3' APOSTROPH RCB 'HexCer' | 'S' ROB '3' APOSTROPH RCB 'GalCer';
acer_hg : acer_hg_pure ROB med RCB;
acer_hg_pure : 'ACer';
acer_species : 'ACer(FA)';
/*
acer_hg : acer_hg_pure | acer_med '-' acer_hg_pure;
acer_med : med;
*/



/* sterol lipids */
sterol : st | st_ester;
st : st_hg headgroup_separator fatty_acyl_chain;
st_ester : st_hg_ester headgroup_separator fatty_acyl_chain sorted_fa_separator fatty_acyl_chain;
st_hg : 'ST' | 'BA' | 'FC' | 'CE' | 'SG' | 'ASG';
st_hg_ester : 'SE';



/* separators */
SPACE : ' ';
COLON : ':';
SEMICOLON : ';';
DASH : '-';
UNDERSCORE : '_';
SLASH : '/';
BACKSLASH : '\\';
COMMA: ',';
ROB: '(';
RCB: ')';
SOB: '[';
SCB: ']';

APOSTROPH : '\'' | '???';
sorted_fa_separator : SLASH;
adduct_separator : SPACE;
unsorted_fa_separator : UNDERSCORE;
plasmalogen_separator : DASH;
headgroup_separator : SPACE;
carbon_db_separator : COLON;
cycle_separator: DASH;
db_funcgroup_separator : SEMICOLON;
db_position_separator : COMMA;
carbohydrate_separator : DASH;
funcgroup_separator : SEMICOLON;
func_repetition_separator : COMMA;
round_open_bracket : ROB;
round_close_bracket : RCB;

number :  digit | digit number;
digit : '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9';

element: 'C' | 'H' | 'N' | 'O' | 'P' | 'S' | 'Br' | 'I' | 'F' | 'Cl' | 'As';
charge : '1' | '2' | '3' | '4';
charge_sign : plus_minus;
plus_minus : '-' | '+';
