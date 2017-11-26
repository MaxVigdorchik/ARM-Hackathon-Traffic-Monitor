import setuptools
import mimir


setuptools.setup(
        name='valhalla-mimir',
        version=mimir.__version__,
        description='Valhalla - Mimir translator',
        long_description=open('README.md').read().strip(),
        author=mimir.__author__,
        author_email=mimir.__email__,
        py_modules=['mimir'],
        install_requires=['paho-mqtt'],
)
