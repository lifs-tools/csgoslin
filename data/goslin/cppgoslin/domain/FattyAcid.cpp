/*
MIT License

Copyright (c) the authors (listed in global LICENSE file)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/


#include "FattyAcid.h"

FattyAcid::FattyAcid(string _name, int _num_carbon, DoubleBonds* _double_bonds, map<string, vector<FunctionalGroup*> >* _functional_groups, LipidFaBondType _lipid_FA_bond_type, bool _lcb, int _position) : FunctionalGroup(_name, _position, 1, _double_bonds, false, "", 0, _functional_groups) {
    
    num_carbon = _num_carbon;
    lipid_FA_bond_type = _lipid_FA_bond_type;
    lcb = _lcb;
    
    if (num_carbon < 0 || num_carbon == 1){
        throw ConstraintViolationException("FattyAcid must have at least 2 carbons! Got " + std::to_string(num_carbon));
    }
    
    if (position < 0){
        throw ConstraintViolationException("FattyAcid position must be greater or equal to 0! Got " + std::to_string(position));
    }
    
    if (double_bonds->get_num() < 0){
        throw ConstraintViolationException("FattyAcid must have at least 0 double bonds! Got " + std::to_string(double_bonds->get_num()));
    }
}


FattyAcid* FattyAcid::copy(){
    
    DoubleBonds* db = double_bonds->copy();
    map<string, vector<FunctionalGroup*> >* fg = new map<string, vector<FunctionalGroup*> >();
    for (auto &kv : *functional_groups){
        fg->insert({kv.first, vector<FunctionalGroup*>()});
        for (auto &func_group : kv.second) {
            fg->at(kv.first).push_back(func_group->copy());
        }
    }
    
    return new FattyAcid(name, num_carbon, db, fg, lipid_FA_bond_type, lcb, position);
}



string FattyAcid::get_prefix(LipidFaBondType lipid_FA_bond_type){
    switch(lipid_FA_bond_type){
        case (ETHER_PLASMANYL): return "O-";
        case (ETHER_PLASMENYL): return "P-";
        default: return "";
    }
}



int FattyAcid::get_double_bonds(){
    return FunctionalGroup::get_double_bonds() + (lipid_FA_bond_type == ETHER_PLASMENYL);
}
    


bool FattyAcid::lipid_FA_bond_type_prefix(LipidFaBondType lipid_FA_bond_type){
    return (lipid_FA_bond_type == ETHER_PLASMANYL) || (lipid_FA_bond_type == ETHER_PLASMENYL) || (lipid_FA_bond_type == ETHER_UNSPECIFIED); 
}



string FattyAcid::to_string(LipidLevel level){
    stringstream fa_string;
    fa_string << get_prefix(lipid_FA_bond_type);
    int num_carbons = num_carbon;
    int num_double_bonds = double_bonds->get_num();
    
    if (num_carbons == 0 && num_double_bonds == 0 && level != ISOMERIC_SUBSPECIES && level != STRUCTURAL_SUBSPECIES){
        return "";
    }
    
    if (level == MOLECULAR_SUBSPECIES){
        ElementTable* e = get_elements();
        num_carbons = e->at(ELEMENT_C);
        num_double_bonds = get_double_bonds() - (lipid_FA_bond_type == ETHER_PLASMENYL);
        delete e;
    }
    

    fa_string << num_carbons << ":" << num_double_bonds;
    
    
    if (level != MOLECULAR_SUBSPECIES && double_bonds->double_bond_positions.size() > 0){
        fa_string << "(";
        
        int i = 0;
        for (auto &kv : double_bonds->double_bond_positions){
            if (i++ > 0) fa_string << ",";
            fa_string << kv.first;
            if (level == ISOMERIC_SUBSPECIES) fa_string << kv.second;
        }
        fa_string << ")";
    }
        
        
    if (level == ISOMERIC_SUBSPECIES){
        vector<string> fg_names;
        for (auto &kv : *functional_groups) fg_names.push_back(kv.first);
        sort(fg_names.begin(), fg_names.end(), lower_name_sort_function);
        
        for (auto &fg : fg_names){
            vector<FunctionalGroup*>& fg_list = functional_groups->at(fg);
            if (fg_list.empty()) continue;
            
            sort(fg_list.begin(), fg_list.end(), FunctionalGroup::position_sort_function);
            int i = 0;
            fa_string << ";";
            for (auto func_group : fg_list){
                if (i++ > 0) fa_string << ",";
                fa_string << func_group->to_string(level);
            }
        }
    }
    
    else if (level == STRUCTURAL_SUBSPECIES){
        vector<string> fg_names;
        for (auto &kv : *functional_groups) fg_names.push_back(kv.first);
        sort(fg_names.begin(), fg_names.end(), lower_name_sort_function);
        
        
        for (auto &fg : fg_names){
            vector<FunctionalGroup*> &fg_list = functional_groups->at(fg);
            if (fg_list.empty()) continue;
                
            if (contains(fg_exceptions, fg)){
                fa_string << ";";
                int i = 0;
                for (auto func_group : fg_list){
                    if (i++ > 0) fa_string << ",";
                    fa_string << func_group->to_string(level);
                }
            }
            
            else {
                int fg_count = 0;
                for (auto func_group : fg_list) fg_count += func_group->count;
                
                
                if (fg_count > 1){
                    fa_string << ";" << (!fg_list.at(0)->is_atomic ? ("(" + fg + ")" + std::to_string(fg_count)) : (fg + std::to_string(fg_count)));
                }
                else {
                    fa_string << ";" << fg;
                }
            }
        }
                        
    }
    else {
        ElementTable *elements = get_functional_group_elements();
        for (int i = 2; i < (int)element_order.size(); ++i){
            Element e = element_order.at(i);
            if (elements->at(e) > 0){
                fa_string << ";" << element_shortcut.at(e);
                if (elements->at(e) > 1){
                    fa_string << elements->at(e);
                }
            }
        } 
        delete elements;
    }
    return fa_string.str();
}




void FattyAcid::compute_elements(){
    for (auto &kv : *elements) elements->at(kv.first) = 0;
    
    int num_double_bonds = double_bonds->num_double_bonds;
    if (lipid_FA_bond_type == ETHER_PLASMENYL) num_double_bonds += 1;
    
    if (num_carbon == 0 && num_double_bonds == 0){
        elements->at(ELEMENT_H) = 1;
        return;
    }
    
    if (!lcb){
        
        elements->at(ELEMENT_C) = num_carbon; // carbon
        if (lipid_FA_bond_type == ESTER){
            elements->at(ELEMENT_H) = (2 * num_carbon - 1 - 2 * num_double_bonds); // hydrogen
            elements->at(ELEMENT_O) = 1; // oxygen
        }
        
        else if (lipid_FA_bond_type == ETHER_PLASMENYL) {
            elements->at(ELEMENT_H) = (2 * num_carbon - 1 - 2 * num_double_bonds + 2); // hydrogen
        }
        
        else if (lipid_FA_bond_type == ETHER_PLASMANYL){
            elements->at(ELEMENT_H) = ((num_carbon + 1) * 2 - 1 - 2 * num_double_bonds); // hydrogen
        }
        
        else if (lipid_FA_bond_type == AMINE)
            elements->at(ELEMENT_H) = (2 * num_carbon + 1 - 2 * num_double_bonds); // hydrogen
            
        else {
            throw LipidException("Mass cannot be computed for fatty acyl chain with this bond type");
        }
    }
    else {
        // long chain base
        elements->at(ELEMENT_C) = num_carbon; // carbon
        elements->at(ELEMENT_H) = (2 * (num_carbon - num_double_bonds) + 1); // hydrogen
        elements->at(ELEMENT_N) = 1; // nitrogen
        elements->at(ELEMENT_O) = 1; // oxygen
    }
}




AcylAlkylGroup::AcylAlkylGroup(FattyAcid* _fa, int _position, int _count, bool _alkyl, bool _N_bond) : FunctionalGroup("O", _position, _count){
    alkyl = _alkyl;
    if (_fa != 0){
        functional_groups->insert({alkyl ? "alkyl" : "acyl", vector<FunctionalGroup*> {_fa} });
    }
    double_bonds->num_double_bonds = int(!alkyl);
    set_N_bond_type(_N_bond);
    
}


AcylAlkylGroup* AcylAlkylGroup::copy(){
    return new AcylAlkylGroup((FattyAcid*)functional_groups->at(alkyl ? "alkyl" : "acyl").at(0)->copy(), position, count, alkyl, N_bond);
}


void AcylAlkylGroup::set_N_bond_type(bool _N_bond){
    N_bond = _N_bond;
        
    if (N_bond){
        elements->at(ELEMENT_H) = alkyl ? 2 : 0;
        elements->at(ELEMENT_O) = alkyl ? -1 : 0;
        elements->at(ELEMENT_N) = 1;
    }
    else {
        elements->at(ELEMENT_H) = alkyl ? 1 : -1;
        elements->at(ELEMENT_O) = alkyl ? 0 : 1;
    }
}


string AcylAlkylGroup::to_string(LipidLevel level){
    stringstream acyl_alkyl_string;
    if (level == ISOMERIC_SUBSPECIES) acyl_alkyl_string << position;
    acyl_alkyl_string << (N_bond ? "N" : "O") << "(";
    if (!alkyl) acyl_alkyl_string << "FA ";
    acyl_alkyl_string << ((FattyAcid*)functional_groups->at(alkyl ? "alkyl" :"acyl").front())->to_string(level) << ")";
    
    return acyl_alkyl_string.str();
}




CarbonChain::CarbonChain(FattyAcid* _fa, int _position, int _count) : FunctionalGroup("cc", _position, _count){
    if (_fa != 0){
        functional_groups->insert({"cc", vector<FunctionalGroup*> {_fa}});
    }
    
    elements->at(ELEMENT_H) = 1;
    elements->at(ELEMENT_O) = -1;
}

CarbonChain* CarbonChain::copy(){
    return new CarbonChain((FattyAcid*)functional_groups->at("cc").at(0)->copy(), position, count);
}


string CarbonChain::to_string(LipidLevel level){
    return (level == ISOMERIC_SUBSPECIES ? std::to_string(position) : "") + "(" + ((FattyAcid*)functional_groups->at("cc").front())->to_string(level) + ")";
}