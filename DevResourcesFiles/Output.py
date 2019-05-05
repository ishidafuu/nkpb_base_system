# -*- coding: utf-8 -*-
import os
import os.path
import sys
import shutil
import subprocess

aseprite_ext = '.aseprite'


def listup_files():
    print('listup_files')
    # ディレクトリ取得
    res = {}
    datadir = (os.path.realpath(os.path.dirname(__file__)) + "\\")
    allfiles = os.listdir(datadir)
    for item in allfiles:
        file_name, ext = os.path.splitext(os.path.basename(item))

        if (ext != aseprite_ext):
            continue

        file_name_sprit = file_name.split('_')
        if (len(file_name_sprit) < 2):
            continue

        base_name = file_name_sprit[0]
        ani_name = [file_name_sprit[1]]

        if ((base_name in res) == False):
            res[base_name] = []

        res[base_name].extend(ani_name)

    for item in res.keys():
        print(item)

    return res

    # aseprite(file, ext)
    # texturePacker(file)


def aseprite(file_dict):
    print('aseprite')
    for item in file_dict.keys():
        base_name = item
        for ani in file_dict[item]:
            file_name = base_name + '_' + ani
            output_dir = base_name
            cmd = 'aseprite.exe -b'
            cmd += ' --split-layers ' + file_name + aseprite_ext
            cmd += ' --save-as ' + output_dir + '/' + file_name + '-{layer}-{frame000}.png'
            cmd += ' --list-layers'
            cmd += ' --list-tags'
            cmd += ' --data ' + output_dir + '/' + file_name + '.json'
            print(cmd)
            subprocess.call(cmd.split())


def texturePacker(file_dict):
    print('texturePacker')
    for item in file_dict.keys():
        base_name = item
        ani_name = file_dict[item]
        source_dir = base_name
        cmd = 'TexturePacker'
        cmd += ' --data ' + base_name + '.tpsheet'
        cmd += ' --sheet ' + base_name + '.png '
        cmd += source_dir
        cmd += ' setting.tps'
        print(cmd)
        subprocess.call(cmd.split())


def rmtree_output(file_dict):
    for item in file_dict.keys():
        if (os.path.exists(item)):
            shutil.rmtree(item)


def remove_background(file_dict):
    for item in file_dict.keys():
        if (os.path.exists(item)):
            allfiles = os.listdir(item)
            for file in allfiles:
                if (file.find('Background') != -1):
                    path = item + '/' + file
                    os.remove(path)


file_dict = listup_files()
rmtree_output(file_dict)
aseprite(file_dict)
remove_background(file_dict)
texturePacker(file_dict)
key = input('!!Kanryou!!')
